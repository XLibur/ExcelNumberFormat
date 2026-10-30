using System;
using System.Globalization;

namespace ExcelNumberFormat
{
    internal enum FormatValueKind : byte
    {
        /// <summary>Any numeric type, held as a double.</summary>
        Number,
        DateTime,
        TimeSpan,
        Text,

        /// <summary>A value that has no format, such as a bool. It is shown as its own text.</summary>
        Other,
    }

    /// <summary>
    /// A value to format. Numbers, dates and durations are held in the struct, so formatting a typed value does not box it.
    /// All numeric types are mapped onto a double and DateOnly/TimeOnly onto a DateTime/double, as the formatter works on those.
    /// </summary>
    internal readonly struct FormatValue
    {
        private readonly double number;
        private readonly long ticks;
        private readonly DateTimeKind dateKind;
        private readonly object other;

        private FormatValue(FormatValueKind kind, double number = 0, long ticks = 0, DateTimeKind dateKind = DateTimeKind.Unspecified, object other = null)
        {
            Kind = kind;
            this.number = number;
            this.ticks = ticks;
            this.dateKind = dateKind;
            this.other = other;
        }

        public FormatValueKind Kind { get; }

        public double Number => number;

        public DateTime DateTime => new DateTime(ticks, dateKind);

        public TimeSpan TimeSpan => new TimeSpan(ticks);

        public static FormatValue FromDouble(double value) => new FormatValue(FormatValueKind.Number, number: value);

        public static FormatValue FromDateTime(DateTime value) => new FormatValue(FormatValueKind.DateTime, ticks: value.Ticks, dateKind: value.Kind);

        public static FormatValue FromTimeSpan(TimeSpan value) => new FormatValue(FormatValueKind.TimeSpan, ticks: value.Ticks);

        public static FormatValue FromSingle(float value) => FromDouble(CompatibleConvert.SingleToDouble(value));

        public static FormatValue FromObject(object value)
        {
            switch (value)
            {
                case string s:
                    return new FormatValue(FormatValueKind.Text, other: s);
                case double d:
                    return FromDouble(d);
                case int i:
                    return FromDouble(i);
                case short sh:
                    return FromDouble(sh);
                case long l:
                    return FromDouble(l);
                case float f:
                    return FromSingle(f);
                case decimal m:
                    return FromDouble((double)m);
                case byte b:
                    return FromDouble(b);
                case sbyte sb:
                    return FromDouble(sb);
                case ushort us:
                    return FromDouble(us);
                case uint ui:
                    return FromDouble(ui);
                case ulong ul:
                    return FromDouble(ul);
                case DateTime dt:
                    return FromDateTime(dt);
                case TimeSpan ts:
                    return FromTimeSpan(ts);
                case DateOnly d:
                    return FromDateTime(d.ToDateTime(TimeOnly.MinValue));
                case TimeOnly t:
                    return FromDouble(t.Ticks / (double)TimeSpan.TicksPerDay);
                default:
                    return new FormatValue(FormatValueKind.Other, other: value);
            }
        }

        /// <summary>
        /// The value as a double, the way Convert.ToDouble does it. Dates and durations have no numeric value.
        /// </summary>
        public double ToDouble(IFormatProvider provider)
        {
            switch (Kind)
            {
                case FormatValueKind.Number:
                    return number;
                case FormatValueKind.Text:
                case FormatValueKind.Other:
                    return Convert.ToDouble(other, provider);
                default:
                    throw new InvalidCastException();
            }
        }

        /// <summary>
        /// The value as plain text, for the General and text sections.
        /// </summary>
        public string ToString(CultureInfo culture)
        {
            switch (Kind)
            {
                case FormatValueKind.Number:
                    return number.ToString("G15", culture);
                case FormatValueKind.DateTime:
                    return DateTime.ToString(culture);
                case FormatValueKind.TimeSpan:
                    return TimeSpan.ToString(null, culture);
                case FormatValueKind.Text:
                    return (string)other;
                default:
                    return Convert.ToString(other, culture);
            }
        }
    }
}
