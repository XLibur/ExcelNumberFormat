using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace ExcelNumberFormat
{
    /// <summary>
    /// Parse ECMA-376 number format strings and format values like Excel and other spreadsheet softwares.
    /// </summary>
    public class NumberFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberFormat"/> class.
        /// </summary>
        /// <param name="formatString">The number format string.</param>
        public NumberFormat(string formatString)
        {
            var sections = Parser.ParseSections(formatString, out bool syntaxError);

            IsValid = !syntaxError;
            FormatString = formatString;

            if (IsValid)
            {
                Sections = sections;
                IsDateTimeFormat = Evaluator.GetFirstSection(Sections, SectionType.Date) != null;
                IsTimeSpanFormat = Evaluator.GetFirstSection(Sections, SectionType.Duration) != null;
            }
            else
            {
                Sections = new List<Section>();
            }
        }

        private const int MaxCachedFormats = 256;

        private static readonly ConcurrentDictionary<string, NumberFormat> Cache = new ConcurrentDictionary<string, NumberFormat>(StringComparer.Ordinal);

        /// <summary>
        /// Returns a number format for the format string, parsing it only the first time it is seen.
        /// </summary>
        /// <remarks>
        /// Parsing costs more than formatting a value, so use this when the same format strings come up again and again,
        /// for example once per cell. A <see cref="NumberFormat"/> is immutable and safe to share between threads.
        /// At most 256 formats are kept; the cache starts over when it is full, so it cannot grow without bound.
        /// </remarks>
        /// <param name="formatString">The number format string.</param>
        public static NumberFormat GetOrCreate(string formatString)
        {
            if (formatString == null)
                return new NumberFormat(formatString);

            if (Cache.TryGetValue(formatString, out var cached))
                return cached;

            var format = new NumberFormat(formatString);
            if (Cache.Count >= MaxCachedFormats)
                Cache.Clear();

            Cache.TryAdd(formatString, format);
            return format;
        }

        /// <summary>
        /// Gets a value indicating whether the number format string is valid.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the number format string.
        /// </summary>
        public string FormatString { get; }

        /// <summary>
        /// Gets a value indicating whether the format represents a DateTime
        /// </summary>
        public bool IsDateTimeFormat { get; }

        /// <summary>
        /// Gets a value indicating whether the format represents a TimeSpan
        /// </summary>
        public bool IsTimeSpanFormat { get; }

        internal List<Section> Sections { get; }

        /// <summary>
        /// Formats a value with this number format in a specified culture.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="culture">The culture to use for formatting.</param>
        /// <param name="isDate1904">If false, numeric dates start on January 0 1900 and include February 29 1900 - like Excel on PC. If true, numeric dates start on January 1 1904 - like Excel on Mac.</param>
        /// <returns>The formatted string.</returns>
        public string Format(object value, CultureInfo culture, bool isDate1904 = false)
        {
            return FormatCore(FormatValue.FromObject(value), culture, isDate1904) ?? CompatibleConvert.ToString(value, culture);
        }

        /// <summary>
        /// Formats a number with this number format in a specified culture, without boxing it.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="culture">The culture to use for formatting.</param>
        /// <param name="isDate1904">If false, numeric dates start on January 0 1900 and include February 29 1900 - like Excel on PC. If true, numeric dates start on January 1 1904 - like Excel on Mac.</param>
        /// <returns>The formatted string.</returns>
        public string Format(double value, CultureInfo culture, bool isDate1904 = false)
        {
            return FormatCore(FormatValue.FromDouble(value), culture, isDate1904) ?? value.ToString("G15", culture);
        }

        /// <inheritdoc cref="Format(double, CultureInfo, bool)"/>
        public string Format(float value, CultureInfo culture, bool isDate1904 = false)
        {
            return FormatCore(FormatValue.FromSingle(value), culture, isDate1904) ?? value.ToString("G7", culture);
        }

        /// <inheritdoc cref="Format(double, CultureInfo, bool)"/>
        public string Format(decimal value, CultureInfo culture, bool isDate1904 = false)
        {
            return FormatCore(FormatValue.FromDouble((double)value), culture, isDate1904) ?? value.ToString(culture);
        }

        /// <inheritdoc cref="Format(double, CultureInfo, bool)"/>
        public string Format(long value, CultureInfo culture, bool isDate1904 = false)
        {
            return FormatCore(FormatValue.FromDouble(value), culture, isDate1904) ?? value.ToString(culture);
        }

        /// <inheritdoc cref="Format(double, CultureInfo, bool)"/>
        /// <remarks>
        /// Without this overload a ulong would bind to the float overload and lose precision.
        /// </remarks>
        [CLSCompliant(false)]
        public string Format(ulong value, CultureInfo culture, bool isDate1904 = false)
        {
            return FormatCore(FormatValue.FromDouble(value), culture, isDate1904) ?? value.ToString(culture);
        }

        /// <summary>
        /// Formats a date and time with this number format in a specified culture, without boxing it.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="culture">The culture to use for formatting.</param>
        /// <param name="isDate1904">Not used for a DateTime, which is not a numeric date.</param>
        /// <returns>The formatted string.</returns>
        public string Format(DateTime value, CultureInfo culture, bool isDate1904 = false)
        {
            return FormatCore(FormatValue.FromDateTime(value), culture, isDate1904) ?? value.ToString(culture);
        }

        /// <summary>
        /// Formats a duration with this number format in a specified culture, without boxing it.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="culture">The culture to use for formatting.</param>
        /// <param name="isDate1904">Not used for a TimeSpan.</param>
        /// <returns>The formatted string.</returns>
        public string Format(TimeSpan value, CultureInfo culture, bool isDate1904 = false)
        {
            return FormatCore(FormatValue.FromTimeSpan(value), culture, isDate1904) ?? value.ToString(null, culture);
        }

        /// <summary>
        /// Formats a value, or returns null if the format has no section for it or cannot be applied to it,
        /// in which case the caller shows the value as plain text.
        /// </summary>
        private string FormatCore(FormatValue value, CultureInfo culture, bool isDate1904)
        {
            var section = Evaluator.GetSection(Sections, value);
            if (section == null)
                return null;

            try
            {
                return Formatter.Format(value, section, culture, isDate1904);
            }
            catch (InvalidCastException)
            {
                // TimeSpan cast exception
                return null;
            }
            catch (FormatException)
            {
                // Convert.ToDouble/ToDateTime exceptions
                return null;
            }
            catch (OverflowException)
            {
                // Values out of range for the format, such as an infinite duration
                return null;
            }
            catch (ArgumentException)
            {
                // Includes ArgumentOutOfRangeException from DateTime and TimeSpan
                return null;
            }
        }
    }
}
