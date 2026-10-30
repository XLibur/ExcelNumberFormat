using System;
using System.Globalization;

namespace ExcelNumberFormat
{
    internal static class CompatibleConvert
    {
        /// <summary>
        /// Widens a float to a double. Whole numbers that print as integers widen exactly, 123456792f stays 123456792.
        /// Others go through a short string, so 1234.56f becomes 1234.56 and not 1234.56005859375.
        /// Very large floats keep 7 significant digits, as General does.
        /// </summary>
        public static double SingleToDouble(float f)
        {
            if (float.IsNaN(f) || float.IsInfinity(f) || (f == Math.Floor(f) && Math.Abs(f) < 1e15f))
                return f;

            var format = f == Math.Floor(f) ? "G7" : "R";
            return double.Parse(f.ToString(format, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// A backward-compatible version of <see cref="Convert.ToString(object, IFormatProvider)"/>.
        /// Starting from .net Core 3.0 the default precision used for formatting floating point number has changed.
        /// To always format numbers the same way, no matter what version of runtime is used, we specify the precision explicitly.
        /// </summary>
        public static string ToString(object value, IFormatProvider provider)
        {
            switch (value)
            {
                case double d:
                    return d.ToString("G15", provider);
                case float f:
                    return f.ToString("G7", provider);
                default:
                    return Convert.ToString(value, provider);
            }
        }
    }
}