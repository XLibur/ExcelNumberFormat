using System;

namespace ExcelNumberFormat
{
    /// <summary>
    /// An era of the Japanese calendar, which Excel uses for the "g" and "e" date codes with the [$-411] locale.
    /// The table is built in, so the result does not depend on the globalization data of the platform.
    /// </summary>
    internal readonly struct JapaneseEra
    {
        private JapaneseEra(DateTime start, string name, string shortName, string initial)
        {
            Start = start;
            Name = name;
            ShortName = shortName;
            Initial = initial;
        }

        public DateTime Start { get; }

        public int StartYear => Start.Year;

        /// <summary>
        /// The full name, printed by "ggg".
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The first character of the name, printed by "gg".
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// The Latin initial, printed by "g".
        /// </summary>
        public string Initial { get; }

        // Newest first. The names are Meiji, Taisho, Showa, Heisei and Reiwa.
        private static readonly JapaneseEra[] Eras =
        {
            new JapaneseEra(new DateTime(2019, 5, 1), "令和", "令", "R"),
            new JapaneseEra(new DateTime(1989, 1, 8), "平成", "平", "H"),
            new JapaneseEra(new DateTime(1926, 12, 25), "昭和", "昭", "S"),
            new JapaneseEra(new DateTime(1912, 7, 30), "大正", "大", "T"),
            new JapaneseEra(new DateTime(1868, 1, 1), "明治", "明", "M"),
        };

        /// <summary>
        /// Finds the era of a date. Returns false for dates before the Meiji era, which Excel cannot hold.
        /// </summary>
        public static bool TryGet(DateTime date, out JapaneseEra era)
        {
            foreach (var candidate in Eras)
            {
                if (date >= candidate.Start)
                {
                    era = candidate;
                    return true;
                }
            }

            era = default;
            return false;
        }
    }
}
