using System;

namespace ExcelNumberFormat
{
    internal static class Token
    {
        /// <summary>
        /// True for the ".0", ".00" and ".000" sub-second tokens.
        /// </summary>
        public static bool IsSubSecond(string token)
        {
            return token.Length > 1 && token[0] == '.' && token[1] == '0';
        }

        public static bool IsExponent(string token)
        {
            return
                (string.Compare(token, "e+", StringComparison.OrdinalIgnoreCase) == 0) ||
                (string.Compare(token, "e-", StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static bool IsLiteral(string token)
        {
            if (token.Length == 0)
                return false;

            // Escapes, quoted text, fill and padding start a longer token
            switch (token[0])
            {
                case '_':
                case '\\':
                case '"':
                case '*':
                    return true;
            }

            if (token.Length != 1)
                return false;

            switch (token[0])
            {
                case ',':
                case '!':
                case '&':
                case '%':
                case '+':
                case '-':
                case '$':
                case '€':
                case '£':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '{':
                case '}':
                case '(':
                case ')':
                case ' ':
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumberLiteral(string token)
        {
            return
                IsPlaceholder(token) ||
                IsLiteral(token) ||
                token == ".";
        }

        public static bool IsPlaceholder(string token)
        {
            return token == "0" || token == "#" || token == "?";
        }

        public static bool IsGeneral(string token)
        {
            return string.Compare(token, "general", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool IsDatePart(string token)
        {
            if (token.Length == 0)
                return false;

            switch (char.ToLowerInvariant(token[0]))
            {
                case 'y':
                case 'm':
                case 'd':
                case 's':
                case 'h':
                    return true;
                case 'g':
                    return !IsGeneral(token);
                case 'a':
                    return string.Equals(token, "am/pm", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(token, "a/p", StringComparison.OrdinalIgnoreCase);
                case '[':
                    return IsDurationPart(token);
                default:
                    return false;
            }
        }

        public static bool IsDurationPart(string token)
        {
            if (token.Length < 2 || token[0] != '[')
                return false;

            switch (char.ToLowerInvariant(token[1]))
            {
                case 'h':
                case 'm':
                case 's':
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDigit09(string token)
        {
            return token == "0" || IsDigit19(token);
        }

        public static bool IsDigit19(string token)
        {
            switch (token)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    return true;
                default:
                    return false;
            }
        }
    }
}
