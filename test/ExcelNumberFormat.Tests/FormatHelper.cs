using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Xlibur.ExcelNumberFormat.Tests
{
    internal static class FormatHelper
    {
        public static string Format(object value, string formatString, CultureInfo culture, bool isDate1904 = false)
        {
            var format = new NumberFormat(formatString);
            if (format.IsValid)
                return format.Format(value, culture, isDate1904);

            return null;
        }

        public static bool IsDateFormatString(string formatString)
        {
            var format = new NumberFormat(formatString);
            return format?.IsDateTimeFormat ?? false;
        }

        public static async Task Test(object value, string format, string expected, bool isDate1904 = false)
        {
            var result = Format(value, format, CultureInfo.InvariantCulture, isDate1904);
            await Assert.That(result).IsEqualTo(expected);
        }
    }
}
