using System;
using System.Globalization;
using System.Threading.Tasks;
using static ExcelNumberFormat.Tests.FormatHelper;

// The expected values were produced by Excel 16 with the same format strings.

namespace ExcelNumberFormat.Tests
{
    public class AmPmTests
    {
        private static readonly DateTime Morning = new DateTime(2020, 1, 1, 6, 0, 0);
        private static readonly DateTime Evening = new DateTime(2020, 1, 1, 18, 0, 0);

        [Test]
        public async Task TestLocaleCodeSelectsDesignators()
        {
            // https://github.com/andersnm/ExcelNumberFormat/issues/45
            await Test(Morning, "[$-405]h:mm AM/PM", "6:00 dop.");
            await Test(Evening, "[$-405]h:mm AM/PM", "6:00 odp.");
            await Test(Morning, "[$-405]h:mm am/pm", "6:00 dop.");
            await Test(Morning, "[$-411]h:mm AM/PM", "6:00 午前");
            await Test(Evening, "[$-411]h:mm AM/PM", "6:00 午後");
            await Test(Morning, "[$-804]h:mm AM/PM", "6:00 上午");
            await Test(Evening, "[$-804]h:mm AM/PM", "6:00 下午");
            await Test(Evening, "[$-409]h:mm AM/PM", "6:00 PM");
        }

        [Test]
        public async Task TestLocaleCodeAlsoAppliesToShortDesignator()
        {
            // Excel only keeps the letters of A/P when the format has no locale code
            await Test(Morning, "[$-405]h:mm A/P", "6:00 dop.");
            await Test(Evening, "[$-405]h:mm a/p", "6:00 odp.");
            await Test(Evening, "[$-409]h:mm A/P", "6:00 PM");
            await Test(Morning, "[$-407]h:mm a/p", "6:00 AM");
        }

        [Test]
        public async Task TestCultureSelectsDesignatorsWithoutLocaleCode()
        {
            var cs = new CultureInfo("cs-CZ");
            await Assert.That(Format(Morning, "h:mm AM/PM", cs)).IsEqualTo("6:00 " + cs.DateTimeFormat.AMDesignator);
            await Assert.That(Format(Evening, "h:mm AM/PM", cs)).IsEqualTo("6:00 " + cs.DateTimeFormat.PMDesignator);
            await Assert.That(Format(Evening, "h:mm A/P", cs)).IsEqualTo("6:00 P");
            await Assert.That(Format(Evening, "h:mm a/p", cs)).IsEqualTo("6:00 p");
        }

        [Test]
        public async Task TestLocaleCodeWinsOverCulture()
        {
            var cs = new CultureInfo("cs-CZ");
            await Assert.That(Format(Evening, "[$-409]h:mm AM/PM", cs)).IsEqualTo("6:00 PM");
            await Assert.That(Format(Evening, "[$-1010409]h:mm AM/PM", cs)).IsEqualTo("6:00 PM");
        }
    }
}
