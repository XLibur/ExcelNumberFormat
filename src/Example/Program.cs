using System;
using System.Globalization;
using ExcelNumberFormat;

namespace Example
{
    class Program
    {
        static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        static void Main(string[] args)
        {
            Section("Numbers");
            Show(1234.5678, "0.00");
            Show(1234.5678, "#,##0.00");
            Show(0.256, "0.0%");
            Show(2.5, "0");                  // rounds half away from zero, as Excel does
            Show(1234567, "#,##0,\"K\"");    // trailing comma scales by 1000
            Show(1234.5, "General");
            Show(42, "000000");

            Section("Sections, conditions and colours");
            Show(5, "0.0;-0.0;\"zero\"");
            Show(-5, "0.0;(0.0);\"zero\"");
            Show(0, "0.0;-0.0;\"zero\"");
            Show(150, "[>100]\"big\";[<=100]\"small\"");
            Show(-3.5, "[Red]0.0;[Blue]0.0");   // colour tags are parsed but never appear in the text
            Show("text", "0;0;0;\"Text: \"@");
            Show(12, ";;;");                 // hidden

            Section("Scientific and fractions");
            Show(12345.678, "0.00E+00");
            Show(0.12, "# ??/??");
            Show(3.75, "# ?/4");

            Section("Dates and times");
            var serial = 45498.75;           // 2024-07-25 18:00
            Show(serial, "yyyy-mm-dd hh:mm:ss");
            Show(serial, "dddd, d mmmm yyyy");
            Show(serial, "h:mm AM/PM");
            Show(serial, "h:mm A/P");
            Show(45498.00093, "mm:ss.000");
            Show(new DateTime(2024, 7, 25, 6, 30, 0), "d-mmm-yy h:mm");
            Show(1, "yyyy-mm-dd", isDate1904: true);

            Section("Durations");
            Show(TimeSpan.FromHours(30.5), "[h]:mm:ss");
            Show(TimeSpan.FromSeconds(3725.25), "[mm]:ss.00");
            Show(1.5, "[h]:mm");

            Section("Cultures and locale codes");
            var fr = new CultureInfo("fr-FR");
            var de = new CultureInfo("de-DE");
            Show(1234.5, "#,##0.00", de);
            Show(serial, "dddd d mmmm yyyy", fr);
            Show(serial, "[$-405]h:mm AM/PM");             // Czech designators
            Show(serial, "[$-F800]dddd, mmmm dd, yyyy");  // system long date, rest of the format is ignored
            Show(serial, "[$-F400]h:mm:ss AM/PM");        // system long time
            Show(44259, "[$-411]ggge\"年\"m\"月\"d\"日\""); // Japanese era
            Show(1234.5, "[$$-409]#,##0.00");
            Show(1234.5, "[$€-407] #,##0.00");

            Section("Other value types");
            Show((byte)200, "0.0");
            Show(123456789L, "#,##0");
            Show(1234.56m, "0.0");
            Show(1234.56f, "0.00");
            Show(new DateOnly(2024, 7, 25), "yyyy-mm-dd");
            Show(new TimeOnly(18, 30), "h:mm AM/PM");

            Section("Inspecting a format");
            foreach (var code in new[] { "0.00", "yyyy-mm-dd", "[h]:mm", "0.0;;", "[<]0" })
            {
                var f = new NumberFormat(code);
                Console.WriteLine($"{code,-14} valid={f.IsValid,-5} date={f.IsDateTimeFormat,-5} duration={f.IsTimeSpanFormat}");
            }

            Section("Reusing parsed formats");
            // GetOrCreate caches parsed formats, which pays off when the same code is used for many cells
            var cached = NumberFormat.GetOrCreate("#,##0.00");
            Console.WriteLine(ReferenceEquals(cached, NumberFormat.GetOrCreate("#,##0.00")));
            Console.WriteLine(cached.Format(1234.5, Invariant));   // typed overload, no boxing

            Section("Invalid formats");
            // Format returns the value as text instead of throwing
            Show(1234.5, "[<]0");
        }

        static void Section(string title)
        {
            Console.WriteLine();
            Console.WriteLine("== " + title);
        }

        static void Show(object value, string code, CultureInfo culture = null, bool isDate1904 = false)
        {
            var format = new NumberFormat(code);
            var result = format.Format(value, culture ?? Invariant, isDate1904);
            Console.WriteLine($"{value,-26} {code,-34} -> \"{result}\"");
        }
    }
}
