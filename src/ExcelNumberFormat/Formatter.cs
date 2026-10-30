using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ExcelNumberFormat
{
    static internal class Formatter
    {
        private const int JapaneseLcid = 0x411;

        static public string Format(FormatValue value, Section node, CultureInfo culture, bool isDate1904)
        {
            switch (node.Type)
            {
                case SectionType.Number:
                    return FormatNumber(ToDoubleHidingSign(value, node, culture), node.Number, culture);

                case SectionType.Date:
                    var dateValue = RoundSubSeconds(value, GetFractionDigits(node.GeneralTextDateDurationParts));
                    if (ExcelDateTime.TryConvert(dateValue, isDate1904, culture, out var excelDateTime))
                    {
                        // System long date/time: the rest of the format is ignored
                        if (node.Lcid == 0xF800)
                            return excelDateTime.ToString(culture.DateTimeFormat.LongDatePattern, culture);
                        if (node.Lcid == 0xF400)
                            return excelDateTime.ToString(culture.DateTimeFormat.LongTimePattern, culture);

                        return FormatDate(excelDateTime, node.DateTokens, node.DateContainsAmPm, node.Lcid, culture);
                    }
                    else
                    {
                        throw new FormatException("Unexpected date value");
                    }

                case SectionType.Duration:
                    var durationValue = RoundSubSeconds(value, GetFractionDigits(node.GeneralTextDateDurationParts));
                    if (durationValue.Kind == FormatValueKind.TimeSpan)
                    {
                        return FormatTimeSpan(durationValue.TimeSpan, node.GeneralTextDateDurationParts, culture);
                    }
                    else
                    {
                        var d = durationValue.ToDouble(null);
                        return FormatTimeSpan(TimeSpan.FromDays(d), node.GeneralTextDateDurationParts, culture);
                    }

                case SectionType.General:
                case SectionType.Text:
                    return FormatGeneralText(value.ToString(culture), node.GeneralTextDateDurationParts);

                case SectionType.Exponential:
                    return FormatExponential(ToDoubleHidingSign(value, node, culture), node, culture);

                case SectionType.Fraction:
                    return FormatFraction(ToDoubleHidingSign(value, node, culture), node, culture);

                default:
                    throw new InvalidOperationException("Unknown number format section");
            }
        }

        /// <summary>
        /// Number of fractional second digits requested by a ".0" token, or 0 if there is none. Excel supports up to 3.
        /// </summary>
        static int GetFractionDigits(List<string> tokens)
        {
            var digits = 0;
            foreach (var token in tokens)
            {
                if (Token.IsSubSecond(token))
                    digits = Math.Min(token.Length - 1, 3);
            }

            return digits;
        }

        /// <summary>
        /// Rounds a date, time or duration to the displayed number of fractional second digits (half up, like Excel),
        /// so that a carry ripples into the seconds, minutes and hours.
        /// </summary>
        static FormatValue RoundSubSeconds(FormatValue value, int digits)
        {
            if (digits == 0)
                return value;

            switch (value.Kind)
            {
                case FormatValueKind.Number:
                    return FormatValue.FromDouble(RoundSubSeconds(value.Number, digits));
                case FormatValueKind.DateTime:
                    var dt = value.DateTime;
                    var unit = TicksPerFractionDigit(digits);
                    var roundedTicks = (dt.Ticks + unit / 2) / unit * unit;
                    return roundedTicks > DateTime.MaxValue.Ticks ? value : FormatValue.FromDateTime(new DateTime(roundedTicks, dt.Kind));
                case FormatValueKind.TimeSpan:
                    var ts = value.TimeSpan;
                    var tsUnit = TicksPerFractionDigit(digits);
                    if (ts.Ticks == long.MinValue || Math.Abs(ts.Ticks) > long.MaxValue - tsUnit)
                        return value;

                    var tsTicks = (Math.Abs(ts.Ticks) + tsUnit / 2) / tsUnit * tsUnit;
                    return FormatValue.FromTimeSpan(new TimeSpan(ts.Ticks < 0 ? -tsTicks : tsTicks));
                default:
                    return value;
            }
        }

        static long TicksPerFractionDigit(int digits)
        {
            var unit = TimeSpan.TicksPerSecond;
            for (var i = 0; i < digits; i++)
                unit /= 10;
            return unit;
        }

        /// <summary>
        /// Rounds a numeric day count to a whole number of 10^-digits seconds, half away from zero on the exact value of the double.
        /// Excel does the same: 3661.25 seconds is stored just below .25 and shows as .2, while 12.35 is stored just above and shows as .4.
        /// </summary>
        static double RoundSubSeconds(double value, int digits)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || Math.Abs(value) > 1e7)
                return value;

            // |value| = mantissa * 2^exponent
            var bits = BitConverter.DoubleToInt64Bits(Math.Abs(value));
            var exponent = (int)((bits >> 52) & 0x7FF);
            var mantissa = bits & 0xFFFFFFFFFFFFFL;
            if (exponent == 0)
                exponent++;
            else
                mantissa |= 1L << 52;
            exponent -= 1075;

            // Units per day: 86400 seconds, each split in 10^digits parts. The mantissa has 53 bits and the
            // units per day fit in 27, so the product fits in a UInt128 and nothing is allocated.
            var unitsPerDay = 86400UL * (digits == 1 ? 10UL : digits == 2 ? 100UL : 1000UL);
            var numerator = (UInt128)(ulong)mantissa * unitsPerDay;

            // The guard above keeps the value under 1e7 days, so the exponent is always negative here
            var shift = -exponent;
            ulong units;
            if (shift > 120)
            {
                // The value is far below one unit
                units = 0;
            }
            else
            {
                var denominator = UInt128.One << shift;
                units = (ulong)((numerator * 2 + denominator) / (denominator * 2));
            }

            var rounded = (double)units / (double)unitsPerDay;
            return value < 0 ? -rounded : rounded;
        }

        /// <summary>
        /// Converts the value to a double. The sign is hidden in the second section and in a conditional first section,
        /// where the format itself decides how a negative number is shown.
        /// </summary>
        static double ToDoubleHidingSign(FormatValue value, Section node, CultureInfo culture)
        {
            var number = value.ToDouble(culture);
            if ((node.SectionIndex == 0 && node.Condition != null) || node.SectionIndex == 1)
                number = Math.Abs(number);

            return number;
        }

        static string FormatGeneralText(string text, List<string> tokens)
        {
            var result = StringBuilderCache.Acquire();
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (Token.IsGeneral(token) || token == "@")
                {
                    result.Append(text);
                }
                else
                {
                    FormatLiteral(token, result);
                }
            }
            return StringBuilderCache.GetStringAndRelease(result);
        }

        private static string FormatTimeSpan(TimeSpan timeSpan, List<string> tokens, CultureInfo culture)
        {
            // NOTE/TODO: assumes there is exactly one [hh], [mm] or [ss] using the integer part of TimeSpan.TotalXXX when formatting.
            // The timeSpan input is then truncated to the remainder fraction, which is used to format mm and/or ss.
            var result = StringBuilderCache.Acquire();
            var containsMilliseconds = false;
            for (var i = tokens.Count - 1; i >= 0; i--)
            {
                if (Token.IsSubSecond(tokens[i]))
                {
                    containsMilliseconds = true;
                    break;
                }
            }

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                if (token.StartsWith("m", StringComparison.OrdinalIgnoreCase))
                {
                    var value = timeSpan.Minutes;
                    var digits = token.Length;
                    AppendPadded(result, value, digits);
                }
                else if (token.StartsWith("s", StringComparison.OrdinalIgnoreCase))
                {
                    // If format does not include ms, then include ms in seconds and round before printing
                    var formatMs = containsMilliseconds ? 0 : timeSpan.Milliseconds / 1000D;
                    var value = (int)Math.Round(timeSpan.Seconds + formatMs, 0, MidpointRounding.AwayFromZero);
                    var digits = token.Length;
                    AppendPadded(result, value, digits);
                }
                else if (token.StartsWith("[h", StringComparison.OrdinalIgnoreCase))
                {
                    var value = (long)timeSpan.TotalHours;
                    var digits = token.Length - 2;
                    AppendPadded(result, value, digits);
                    timeSpan = new TimeSpan(0, 0, Math.Abs(timeSpan.Minutes), Math.Abs(timeSpan.Seconds), Math.Abs(timeSpan.Milliseconds));
                }
                else if (token.StartsWith("[m", StringComparison.OrdinalIgnoreCase))
                {
                    var value = (long)timeSpan.TotalMinutes;
                    var digits = token.Length - 2;
                    AppendPadded(result, value, digits);
                    timeSpan = new TimeSpan(0, 0, 0, Math.Abs(timeSpan.Seconds), Math.Abs(timeSpan.Milliseconds));
                }
                else if (token.StartsWith("[s", StringComparison.OrdinalIgnoreCase))
                {
                    var value = (long)timeSpan.TotalSeconds;
                    var digits = token.Length - 2;
                    AppendPadded(result, value, digits);
                    timeSpan = new TimeSpan(0, 0, 0, 0, Math.Abs(timeSpan.Milliseconds));
                }
                else if (Token.IsSubSecond(token)) {
                    AppendSubSeconds(result, timeSpan.Milliseconds, token.Length - 1);
                }
                else
                {
                    FormatLiteral(token, result);
                }
            }

            return StringBuilderCache.GetStringAndRelease(result);
        }

        private static string FormatDate(ExcelDateTime date, DateToken[] tokens, bool containsAmPm, int lcid, CultureInfo culture)
        {
            var result = StringBuilderCache.Acquire();
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];

                switch (token.Kind)
                {
                    case DateTokenKind.Year:
                        AppendPadded(result, token.Digits == 2 ? date.Year % 100 : date.Year, token.Digits);
                        break;

                    case DateTokenKind.Minute:
                        AppendPadded(result, date.Minute, token.Digits);
                        break;

                    case DateTokenKind.Month:
                        if (token.Digits == 3)
                        {
                            result.Append(culture.DateTimeFormat.AbbreviatedMonthNames[date.Month - 1]);
                        }
                        else if (token.Digits == 4)
                        {
                            result.Append(culture.DateTimeFormat.MonthNames[date.Month - 1]);
                        }
                        else if (token.Digits == 5)
                        {
                            result.Append(culture.DateTimeFormat.MonthNames[date.Month - 1][0]);
                        }
                        else
                        {
                            AppendPadded(result, date.Month, token.Digits);
                        }

                        break;

                    case DateTokenKind.Day:
                        if (token.Digits == 3)
                        {
                            // Sun-Sat
                            result.Append(culture.DateTimeFormat.AbbreviatedDayNames[(int)date.DayOfWeek]);
                        }
                        else if (token.Digits == 4)
                        {
                            // Sunday-Saturday
                            result.Append(culture.DateTimeFormat.DayNames[(int)date.DayOfWeek]);
                        }
                        else
                        {
                            AppendPadded(result, date.Day, token.Digits);
                        }

                        break;

                    case DateTokenKind.Hour:
                        AppendPadded(result, containsAmPm ? (date.Hour + 11) % 12 + 1 : date.Hour, token.Digits);
                        break;

                    case DateTokenKind.Second:
                        AppendPadded(result, date.Second, token.Digits);
                        break;

                    case DateTokenKind.Era:
                        if (lcid == JapaneseLcid && JapaneseEra.TryGet(date.AdjustedDateTime, out var japaneseEra))
                        {
                            result.Append(token.Digits == 1 ? japaneseEra.Initial : token.Digits == 2 ? japaneseEra.ShortName : japaneseEra.Name);
                            break;
                        }

                        var era = culture.DateTimeFormat.Calendar.GetEra(date.AdjustedDateTime);
                        if (token.Digits < 3)
                        {
                            result.Append(culture.DateTimeFormat.GetAbbreviatedEraName(era));
                        }
                        else
                        {
                            result.Append(culture.DateTimeFormat.GetEraName(era));
                        }

                        break;

                    case DateTokenKind.EraYear:
                        // Only the Japanese locale counts years from the start of the era, all others print the year
                        if (lcid == JapaneseLcid && JapaneseEra.TryGet(date.AdjustedDateTime, out japaneseEra))
                            AppendPadded(result, date.Year - japaneseEra.StartYear + 1, token.Digits);
                        else
                            AppendPadded(result, date.Year, 4);

                        break;

                    case DateTokenKind.AmPm:
                        result.Append(date.Hour < 12 ? "AM" : "PM");
                        break;

                    case DateTokenKind.AmPmShort:
                        if (token.IsUpper)
                            result.Append(date.Hour < 12 ? 'A' : 'P');
                        else
                            result.Append(date.Hour < 12 ? 'a' : 'p');

                        break;

                    case DateTokenKind.SubSecond:
                        AppendSubSeconds(result, date.Millisecond, token.Digits);
                        break;

                    case DateTokenKind.DateSeparator:
                        result.Append(culture.DateTimeFormat.DateSeparator);
                        break;

                    case DateTokenKind.Comma:
                        result.Append(',');
                        break;

                    default:
                        result.Append(token.Text);
                        break;
                }
            }

            return StringBuilderCache.GetStringAndRelease(result);
        }

        private static readonly long[] PowersOfTen = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };

        private static readonly string[] PaddedFormats = { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9" };

        /// <summary>
        /// Appends a number left-padded with zeros to at least the given number of digits, like ToString("D" + digits) does.
        /// </summary>
        private static void AppendPadded(StringBuilder result, long value, int digits)
        {
            if (value >= 0 && digits < PowersOfTen.Length && value < PowersOfTen[digits])
            {
                for (var place = digits - 1; place >= 0; place--)
                    result.Append((char)('0' + value / PowersOfTen[place] % 10));

                return;
            }

            result.Append(value.ToString(digits < PaddedFormats.Length ? PaddedFormats[digits] : "D" + digits.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Appends the fractional seconds with exactly as many digits as the format asks for.
        /// The value is expected to be rounded already by RoundSubSeconds.
        /// </summary>
        static void AppendSubSeconds(StringBuilder result, int milliseconds, int digits)
        {
            if (milliseconds < 0 || milliseconds > 999)
            {
                result.Append("." + milliseconds.ToString("D3", CultureInfo.InvariantCulture).Substring(0, Math.Min(digits, 3)).PadRight(digits, '0'));
                return;
            }

            result.Append('.');
            for (var i = 0; i < digits; i++)
            {
                result.Append(i == 0 ? (char)('0' + milliseconds / 100) :
                              i == 1 ? (char)('0' + milliseconds / 10 % 10) :
                              i == 2 ? (char)('0' + milliseconds % 10) : '0');
            }
        }

        static string FormatNumber(double value, DecimalSection format, CultureInfo culture)
        {
            bool thousandSeparator = format.ThousandSeparator;
            value = value / format.ThousandDivisor;
            value = value * format.PercentMultiplier;

            var result = StringBuilderCache.Acquire();
            FormatNumber(value, format.BeforeDecimal, format.DecimalSeparator, format.AfterDecimal, thousandSeparator, culture, result);
            return StringBuilderCache.GetStringAndRelease(result);
        }

        static void FormatNumber(double value, List<string> beforeDecimal, bool decimalSeparator, List<string> afterDecimal, bool thousandSeparator, CultureInfo culture, StringBuilder result)
        {
            int signitificantDigits = 0;
            if (afterDecimal != null)
                signitificantDigits = GetDigitCount(afterDecimal);

            var valueString = RoundToFixed(Math.Abs(value), signitificantDigits);
            var decimalPoint = valueString.IndexOf('.');
            var thousandsString = decimalPoint < 0 ? valueString : valueString.Substring(0, decimalPoint);
            var decimalString = decimalPoint < 0 ? "" : valueString.Substring(decimalPoint + 1).TrimEnd('0');

            if (value < 0)
            {
                result.Append("-");
            }

            if (beforeDecimal != null)
            {
                FormatThousands(thousandsString, thousandSeparator, false, beforeDecimal, culture, result);
            }

            if (decimalSeparator) { 
                result.Append(culture.NumberFormat.NumberDecimalSeparator);
            }

            if (afterDecimal != null)
            {
                FormatDecimals(decimalString, afterDecimal, result);
            }
        }

        /// <summary>
        /// Formats a non-negative value with a fixed number of decimals, rounding half away from zero like Excel.
        /// The double is first reduced to 15 significant digits, as Excel does, so 2.675 rounds to 2.68.
        /// </summary>
        // "F0" to "F15", so the common digit counts do not build a format string on every call
        private static readonly string[] FixedFormats =
        {
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15"
        };

        private static readonly double[] Pow10 =
        {
            1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15
        };

        static string FixedFormat(int digits)
        {
            return digits < FixedFormats.Length ? FixedFormats[digits] : "F" + digits.ToString(CultureInfo.InvariantCulture);
        }

        static string RoundToFixed(double value, int digits)
        {
            // Double.ToString("F") is exact, so it only disagrees with Excel's 15 digit rounding at or very near a tie.
            // Away from a tie the plain format gives the same digits and skips the string/decimal round trip below.
            // The scaled value stays under 1e9, which keeps the error in its fractional part well below the margin.
            if (digits < FixedFormats.Length && value < 1e9)
            {
                var scaled = value * Pow10[digits];
                if (scaled < 1e9)
                {
                    var fraction = scaled - Math.Floor(scaled);
                    if (Math.Abs(fraction - 0.5) > 1e-6)
                        return value.ToString(FixedFormats[digits], CultureInfo.InvariantCulture);
                }
            }

            if (digits <= 28 && value < 7.9e28)
            {
                var decimalValue = decimal.Parse(value.ToString("G15", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
                var rounded = Math.Round(decimalValue, digits, MidpointRounding.AwayFromZero);
                return rounded.ToString(FixedFormat(digits), CultureInfo.InvariantCulture);
            }

            return value.ToString(FixedFormat(digits), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Prints right-aligned, left-padded integer before the decimal separator. With optional most-significant zero.
        /// </summary>
        public static void FormatThousands(string valueString, bool thousandSeparator, bool significantZero, List<string> tokens, CultureInfo culture, StringBuilder result)
        {
            var significant = false;
            var formatDigits = GetDigitCount(tokens);
            valueString = valueString.PadLeft(formatDigits, '0');

            // Print literals occurring before any placeholders
            var tokenIndex = 0;
            for (; tokenIndex < tokens.Count; tokenIndex++)
            {
                var token = tokens[tokenIndex];
                if (Token.IsPlaceholder(token))
                    break;
                else
                    FormatLiteral(token, result);
            }

            // Print value digits until there are as many digits remaining as there are placeholders
            var digitIndex = 0;
            for (; digitIndex < (valueString.Length - formatDigits); digitIndex++)
            {
                significant = true;
                result.Append(valueString[digitIndex]);

                if (thousandSeparator)
                    FormatThousandSeparator(valueString, digitIndex, culture, result);
            }

            // Print remaining value digits and format literals
            for (; tokenIndex < tokens.Count; ++tokenIndex)
            {
                var token = tokens[tokenIndex];
                if (Token.IsPlaceholder(token))
                {
                    var c = valueString[digitIndex];
                    if (c != '0' || (significantZero && digitIndex == valueString.Length - 1)) significant = true;

                    FormatPlaceholder(token, c, significant, result);

                    if (thousandSeparator && (significant || token.Equals("0")))
                        FormatThousandSeparator(valueString, digitIndex, culture, result);

                    digitIndex++;
                }
                else
                {
                    FormatLiteral(token, result);
                }
            }
        }

        static void FormatThousandSeparator(string valueString, int digit, CultureInfo culture, StringBuilder result)
        {
            var positionInTens = valueString.Length - 1 - digit;
            if (positionInTens > 0 && (positionInTens % 3) == 0)
            {
                result.Append(culture.NumberFormat.NumberGroupSeparator);
            }
        }

        /// <summary>
        /// Prints left-aligned, right-padded integer after the decimal separator. Does not print significant zero.
        /// </summary>
        public static void FormatDecimals(string valueString, List<string> tokens, StringBuilder result)
        {
            var significant = true;
            var unpaddedDigits = valueString.Length;
            var formatDigits = GetDigitCount(tokens);

            valueString = valueString.PadRight(formatDigits, '0');

            // Print all format digits
            var valueIndex = 0;
            for (var tokenIndex = 0; tokenIndex < tokens.Count; ++tokenIndex)
            {
                var token = tokens[tokenIndex];
                if (Token.IsPlaceholder(token))
                {
                    var c = valueString[valueIndex];
                    significant = valueIndex < unpaddedDigits;

                    FormatPlaceholder(token, c, significant, result);
                    valueIndex++;
                }
                else
                {
                    FormatLiteral(token, result);
                }
            }
        }

        static string FormatExponential(double value, Section format, CultureInfo culture)
        {
            // NaN and infinity have no digits to lay out
            if (double.IsNaN(value) || double.IsInfinity(value))
                return value.ToString(culture);

            // The application shall display a number to the right of 
            // the "E" symbol that corresponds to the number of places that 
            // the decimal point was moved. 

            var baseDigits = 0;
            if (format.Exponential.BeforeDecimal != null)
            {
                baseDigits = GetDigitCount(format.Exponential.BeforeDecimal);
            }

            var exponent = 0;
            var mantissa = value;
            var step = Math.Max(baseDigits, 1);
            var afterDecimalDigits = format.Exponential.AfterDecimal != null ? GetDigitCount(format.Exponential.AfterDecimal) : 0;

            // Zero has no exponent to find
            if (value != 0)
            {
                exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));
                mantissa = value / Math.Pow(10, exponent);

                var shift = baseDigits > 0 ? Math.Abs(exponent) % baseDigits : 0;
                if (shift > 0)
                {
                    if (exponent < 0)
                        shift = (baseDigits - shift);

                    mantissa *= Math.Pow(10, shift);
                    exponent -= shift;
                }

                // Rounding the mantissa can carry it out of its range, 9.999 with 0.00E+00 becomes 10.00.
                // Move to the next exponent, as Excel does: 1.00E+01.
                var rounded = double.Parse(RoundToFixed(Math.Abs(mantissa), afterDecimalDigits), CultureInfo.InvariantCulture);
                if (rounded >= Math.Pow(10, step))
                {
                    mantissa /= Math.Pow(10, step);
                    exponent += step;
                }
            }

            var result = StringBuilderCache.Acquire();
            FormatNumber(mantissa, format.Exponential.BeforeDecimal, format.Exponential.DecimalSeparator, format.Exponential.AfterDecimal, false, culture, result);

            result.Append(format.Exponential.ExponentialToken[0]);

            if (format.Exponential.ExponentialToken[1] == '+' && exponent >= 0)
            {
                result.Append("+");
            }
            else if (exponent < 0)
            {
                result.Append("-");
            }

            FormatThousands(Math.Abs(exponent).ToString(CultureInfo.InvariantCulture), false, false, format.Exponential.Power, culture, result);
            return StringBuilderCache.GetStringAndRelease(result);
        }

        static string FormatFraction(double value, Section format, CultureInfo culture)
        {
            // NaN and infinity have no digits to lay out
            if (double.IsNaN(value) || double.IsInfinity(value))
                return value.ToString(culture);

            // Kept as double, a whole part can be larger than an int
            double integral = 0;
            int numerator, denominator;

            bool sign = value < 0;
            value = Math.Abs(value);

            if (format.Fraction.IntegerPart != null)
            {
                integral = Math.Truncate(value);
                value -= integral;
            }

            if (format.Fraction.DenominatorConstant != 0)
            {
                denominator = format.Fraction.DenominatorConstant;
                var rr = Math.Round(value * denominator);

                // Without an integer part the fraction stays improper (7/4). With one, a carry (3/3) goes to the integer part.
                if (format.Fraction.IntegerPart != null)
                {
                    var carry = Math.Floor(rr / denominator);
                    integral += carry;
                    rr -= carry * denominator;
                }

                numerator = (int)rr;
            }
            else
            {
                var denominatorDigits = Math.Min(GetDigitCount(format.Fraction.Denominator), 7);
                GetFraction(value, (int)Math.Pow(10, denominatorDigits) - 1, out numerator, out denominator);

                if (format.Fraction.IntegerPart != null && numerator >= denominator)
                {
                    integral += numerator / denominator;
                    numerator = 0;
                    denominator = 1;
                }
            }

            // Don't hide fraction if at least one zero in the numerator format
            var numeratorZeros = GetZeroCount(format.Fraction.Numerator);
            var hideFraction = (format.Fraction.IntegerPart != null && numerator == 0 && numeratorZeros == 0);

            var result = StringBuilderCache.Acquire();

            if (sign)
                result.Append("-");

            // Print integer part with significant zero if fraction part is hidden
            if (format.Fraction.IntegerPart != null)
                FormatThousands(integral.ToString("F0", CultureInfo.InvariantCulture), false, hideFraction, format.Fraction.IntegerPart, culture, result);

            var numeratorString = Math.Abs(numerator).ToString("F0", CultureInfo.InvariantCulture);
            var denominatorString = denominator.ToString("F0", CultureInfo.InvariantCulture);

            var fraction = new StringBuilder();

            FormatThousands(numeratorString, false, true, format.Fraction.Numerator, culture, fraction);

            fraction.Append("/");

            if (format.Fraction.DenominatorPrefix != null)
                FormatThousands("", false, false, format.Fraction.DenominatorPrefix, culture, fraction);

            if (format.Fraction.DenominatorConstant != 0)
                fraction.Append(format.Fraction.DenominatorConstant.ToString());
            else
                FormatDenominator(denominatorString, format.Fraction.Denominator, fraction);

            if (format.Fraction.DenominatorSuffix != null)
                FormatThousands("", false, false, format.Fraction.DenominatorSuffix, culture, fraction);

            if (hideFraction)
                result.Append(new string(' ', fraction.ToString().Length));
            else
                result.Append(fraction.ToString());

            if (format.Fraction.FractionSuffix != null)
                FormatThousands("", false, false, format.Fraction.FractionSuffix, culture, result);

            return StringBuilderCache.GetStringAndRelease(result);
        }

        // Adapted from ssf.js 'frac()' helper
        static void GetFraction(double x, int D, out int nom, out int den)
        {
            var sgn = x < 0 ? -1 : 1;
            var B = x * sgn;
            var P_2 = 0.0;
            var P_1 = 1.0;
            var P = 0.0;
            var Q_2 = 1.0;
            var Q_1 = 0.0;
            var Q = 0.0;
            var A = Math.Floor(B);
            while (Q_1 < D)
            {
                A = Math.Floor(B);
                P = A * P_1 + P_2;
                Q = A * Q_1 + Q_2;
                if ((B - A) < 0.00000005) break;
                B = 1 / (B - A);
                P_2 = P_1; P_1 = P;
                Q_2 = Q_1; Q_1 = Q;
            }
            if (Q > D) { if (Q_1 > D) { Q = Q_2; P = P_2; } else { Q = Q_1; P = P_1; } }
            nom = (int)(sgn * P);
            den = (int)Q;
        }

        /// <summary>
        /// Prints left-aligned, left-padded fraction integer denominator.
        /// Assumes tokens contain only placeholders, valueString has fewer or equal number of digits as tokens.
        /// </summary>
        public static void FormatDenominator(string valueString, List<string> tokens, StringBuilder result)
        {
            var formatDigits = GetDigitCount(tokens);
            valueString = valueString.PadLeft(formatDigits, '0');

            bool significant = false;
            var valueIndex = 0;
            for (var tokenIndex = 0; tokenIndex < tokens.Count; ++tokenIndex)
            {
                var token = tokens[tokenIndex];
                char c;
                if (valueIndex < valueString.Length) {
                    c = GetLeftAlignedValueDigit(token, valueString, valueIndex, significant, out valueIndex);

                    if (c != '0')
                        significant = true;
                } else { 
                    c = '0';
                    significant = false;
                }

                FormatPlaceholder(token, c, significant, result);
            }
        }

        /// <summary>
        /// Returns the first digit from valueString. If the token is '?' 
        /// returns the first significant digit from valueString, or '0' if there are no significant digits.
        /// The out valueIndex parameter contains the offset to the next digit in valueString.
        /// </summary>
        static char GetLeftAlignedValueDigit(string token, string valueString, int startIndex, bool significant, out int valueIndex)
        {
            char c;
            valueIndex = startIndex;
            if (valueIndex < valueString.Length)
            {
                c = valueString[valueIndex];
                valueIndex++;

                if (c != '0')
                    significant = true;

                if (token == "?" && !significant)
                {
                    // Eat insignificant zeros to left align denominator
                    while (valueIndex < valueString.Length)
                    {
                        c = valueString[valueIndex];
                        valueIndex++;

                        if (c != '0')
                        {
                            significant = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                c = '0';
                significant = false;
            }

            return c;
        }

        static void FormatPlaceholder(string token, char c, bool significant, StringBuilder result)
        {
            if (token == "0")
            {
                if (significant)
                    result.Append(c);
                else
                    result.Append("0");
            }
            else if (token == "#")
            {
                if (significant)
                    result.Append(c);
            }
            else if (token == "?")
            {
                if (significant)
                    result.Append(c);
                else
                    result.Append(" ");
            }
        }

        static int GetDigitCount(List<string> tokens)
        {
            var counter = 0;
            foreach (var token in tokens)
            {
                if (Token.IsPlaceholder(token))
                {
                    counter++;
                }
            }
            return counter;
        }

        static int GetZeroCount(List<string> tokens)
        {
            var counter = 0;
            foreach (var token in tokens)
            {
                if (token == "0")
                {
                    counter++;
                }
            }
            return counter;
        }

        static void FormatLiteral(string token, StringBuilder result)
        {
            result.Append(GetLiteral(token));
        }

        /// <summary>
        /// The text a literal token prints: commas are skipped, quotes and escapes are removed.
        /// </summary>
        internal static string GetLiteral(string token)
        {
            if (token == ",")
            {
                return string.Empty; // skip commas
            }

            if (token.Length == 2 && (token[0] == '*' || token[0] == '\\'))
            {
                // TODO: * = repeat to fill cell
                return token[1].ToString();
            }

            if (token.Length == 2 && token[0] == '_')
            {
                return " ";
            }

            if (token.Length > 0 && token[0] == '"')
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }
    }
}
