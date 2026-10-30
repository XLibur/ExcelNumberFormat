using System;
using System.Collections.Generic;

namespace ExcelNumberFormat
{
    internal enum DateTokenKind : byte
    {
        Year,
        Month,
        Minute,
        Day,
        Hour,
        Second,
        Era,
        EraYear,
        AmPm,
        AmPmShort,
        SubSecond,
        DateSeparator,
        Comma,
        Literal,
    }

    /// <summary>
    /// A token of a date section, classified once when the format is parsed so that formatting a value
    /// does not have to inspect the token text again.
    /// </summary>
    internal readonly struct DateToken
    {
        public DateToken(DateTokenKind kind, int digits = 0, string text = null, bool isUpper = false)
        {
            Kind = kind;
            Digits = digits;
            Text = text;
            IsUpper = isUpper;
        }

        public DateTokenKind Kind { get; }

        /// <summary>
        /// The number of digits to print, or the number of fractional second digits for a sub-second token.
        /// For a year this is already 2 or 4.
        /// </summary>
        public int Digits { get; }

        /// <summary>
        /// The text to print for a literal.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Whether an "a/p" token was written in upper case.
        /// </summary>
        public bool IsUpper { get; }

        /// <summary>
        /// Classifies the tokens of a date section. Sets <paramref name="containsAmPm"/> if the section uses a 12 hour clock.
        /// </summary>
        public static DateToken[] Create(List<string> tokens, out bool containsAmPm)
        {
            containsAmPm = ContainsAmPm(tokens);

            // An "e-" or "e+" token turns into two date tokens
            var size = tokens.Count;
            for (var i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == "e-" || tokens[i] == "e+")
                    size++;
            }

            var result = new DateToken[size];
            var count = 0;
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                var first = token.Length > 0 ? char.ToLowerInvariant(token[0]) : '\0';

                switch (first)
                {
                    case 'y':
                        // One digit prints as two, and three as four
                        var yearDigits = token.Length < 2 ? 2 : token.Length == 3 ? 4 : token.Length;
                        result[count++] = (new DateToken(DateTokenKind.Year, yearDigits));
                        continue;

                    case 'm':
                        // If "m" or "mm" is used immediately after the "h" or "hh" code (for hours) or immediately before
                        // the "ss" code (for seconds), the application shall display minutes instead of the month.
                        var isMinute = LookBackDatePart(tokens, i - 1, "h") || LookAheadDatePart(tokens, i + 1, "s");
                        result[count++] = (new DateToken(isMinute ? DateTokenKind.Minute : DateTokenKind.Month, token.Length));
                        continue;

                    case 'd':
                        result[count++] = (new DateToken(DateTokenKind.Day, token.Length));
                        continue;

                    case 'h':
                        result[count++] = (new DateToken(DateTokenKind.Hour, token.Length));
                        continue;

                    case 's':
                        result[count++] = (new DateToken(DateTokenKind.Second, token.Length));
                        continue;

                    case 'g':
                        result[count++] = (new DateToken(DateTokenKind.Era, token.Length));
                        continue;

                    case 'e' when token[0] == 'e':
                        // The tokenizer reads "e-" and "e+" as exponents, but in a date they are an era year and a literal
                        result[count++] = (new DateToken(DateTokenKind.EraYear, token.Length == 1 || Token.IsExponent(token) ? 1 : 2));
                        if (Token.IsExponent(token))
                            result[count++] = (new DateToken(DateTokenKind.Literal, text: token.Substring(1)));
                        continue;
                }

                if (string.Equals(token, "am/pm", StringComparison.OrdinalIgnoreCase))
                {
                    result[count++] = (new DateToken(DateTokenKind.AmPm));
                }
                else if (string.Equals(token, "a/p", StringComparison.OrdinalIgnoreCase))
                {
                    result[count++] = (new DateToken(DateTokenKind.AmPmShort, isUpper: char.IsUpper(token[0])));
                }
                else if (Token.IsSubSecond(token))
                {
                    result[count++] = (new DateToken(DateTokenKind.SubSecond, token.Length - 1));
                }
                else if (token == "/")
                {
                    result[count++] = (new DateToken(DateTokenKind.DateSeparator));
                }
                else if (token == ",")
                {
                    // A run of commas prints a single one
                    while (i < tokens.Count - 1 && tokens[i + 1] == ",")
                    {
                        i++;
                    }

                    result[count++] = (new DateToken(DateTokenKind.Comma));
                }
                else
                {
                    result[count++] = (new DateToken(DateTokenKind.Literal, text: Formatter.GetLiteral(token)));
                }
            }

            // Runs of commas collapse into one token, so the array can be shorter than the list of tokens
            if (count < result.Length)
                Array.Resize(ref result, count);

            return result;
        }

        private static bool LookAheadDatePart(List<string> tokens, int fromIndex, string startsWith)
        {
            for (var i = fromIndex; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (Token.IsDatePart(token))
                    return false;
            }

            return false;
        }

        private static bool LookBackDatePart(List<string> tokens, int fromIndex, string startsWith)
        {
            for (var i = fromIndex; i >= 0; i--)
            {
                var token = tokens[i];
                if (token.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (Token.IsDatePart(token))
                    return false;
            }

            return false;
        }

        private static bool ContainsAmPm(List<string> tokens)
        {
            foreach (var token in tokens)
            {
                if (string.Equals(token, "am/pm", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(token, "a/p", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
