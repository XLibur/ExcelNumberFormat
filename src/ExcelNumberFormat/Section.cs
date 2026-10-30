using System.Collections.Generic;

namespace ExcelNumberFormat
{
    internal class Section
    {
        public int SectionIndex { get; set; }

        public SectionType Type { get; set; }

        public Color Color { get; set; }

        public Condition Condition { get; set; }

        public ExponentialSection Exponential { get; set; }

        public FractionSection Fraction { get; set; }

        public DecimalSection Number { get; set; }

        /// <summary>
        /// Locale id from a [$-XXX] token, or 0 if none. 0xF800 and 0xF400 select the system long date and long time.
        /// </summary>
        public int Lcid { get; set; }

        public List<string> GeneralTextDateDurationParts { get; set; }

        /// <summary>
        /// The classified tokens of a date section.
        /// </summary>
        public DateToken[] DateTokens { get; set; }

        /// <summary>
        /// Whether a date section uses a 12 hour clock.
        /// </summary>
        public bool DateContainsAmPm { get; set; }
    }
}