using System;
using System.Globalization;
using System.Threading.Tasks;
using static Xlibur.ExcelNumberFormat.Tests.FormatHelper;

// Much of the test data was adapted from the SheetJS/ssf project.
// Copyright (C) 2013-present  SheetJS. 
// Licensed under the Apache License, Version 2.0

namespace Xlibur.ExcelNumberFormat.Tests
{
    public class DateTests
    {
        [Test]
        public async Task TestIsDateFormatString()
        {
            await Assert.That(IsDateFormatString("dd/mm/yyyy")).IsTrue();
            await Assert.That(IsDateFormatString("dd-mmm-yy")).IsTrue();
            await Assert.That(IsDateFormatString("dd-mmmm")).IsTrue();
            await Assert.That(IsDateFormatString("mmm-yy")).IsTrue();
            await Assert.That(IsDateFormatString("h:mm AM/PM")).IsTrue();
            await Assert.That(IsDateFormatString("h:mm:ss AM/PM")).IsTrue();
            await Assert.That(IsDateFormatString("hh:mm")).IsTrue();
            await Assert.That(IsDateFormatString("hh:mm:ss")).IsTrue();
            await Assert.That(IsDateFormatString("dd/mm/yyyy hh:mm")).IsTrue();
            await Assert.That(IsDateFormatString("mm:ss")).IsTrue();
            await Assert.That(IsDateFormatString("mm:ss.0")).IsTrue();
            await Assert.That(IsDateFormatString("[$-809]dd mmmm yyyy")).IsTrue();
            await Assert.That(IsDateFormatString("#,##0;[Red]-#,##0")).IsFalse();
            await Assert.That(IsDateFormatString("0_);[Red](0)")).IsFalse();
            await Assert.That(IsDateFormatString(@"0\h")).IsFalse();
            await Assert.That(IsDateFormatString("0\"h\"")).IsFalse();
            await Assert.That(IsDateFormatString("0%")).IsFalse();
            await Assert.That(IsDateFormatString("General")).IsFalse();
            await Assert.That(IsDateFormatString(@"_-* #,##0\ _P_t_s_-;\-* #,##0\ _P_t_s_-;_-* "" - ""??\ _P_t_s_-;_-@_- ")).IsFalse();
        }

        [Test]
        public async Task TestDate()
        {
            await Test(new DateTime(2000, 1, 1), "d-mmm-yy", "1-Jan-00");
            await Test(new DateTime(2000, 1, 1, 12, 34, 56), "m/d/yyyy\\ h:mm:ss;@", "1/1/2000 12:34:56");
            await Test(new DateTime(2010, 9, 26), "yyyy-MMM-dd", "2010-Sep-26");
            await Test(new DateTime(2010, 9, 26), "yyyy-MM-dd", "2010-09-26");
            await Test(new DateTime(2010, 9, 26), "mm/dd/yyyy", "09/26/2010");
            await Test(new DateTime(2010, 9, 26), "m/d/yy", "9/26/10");
            await Test(new DateTime(2010, 9, 26, 12, 34, 56, 123), "m/d/yy hh:mm:ss.000", "9/26/10 12:34:56.123");
            await Test(new DateTime(2010, 9, 26, 12, 34, 56, 123), "YYYY-MM-DD HH:MM:SS", "2010-09-26 12:34:56");
            await Test(new DateTime(2020, 1, 1, 14, 35, 55), "m/d/yyyy\\ h:mm:ss AM/PM;@", "1/1/2020 2:35:55 PM");
            await Test(new DateTime(2020, 1, 1, 14, 35, 55), "m/d/yyyy\\ h:mm:ss aM/Pm;@", "1/1/2020 2:35:55 PM");
            await Test(new DateTime(2020, 1, 1, 14, 35, 55), "m/d/yyyy\\ h:mm:ss am/PM;@", "1/1/2020 2:35:55 PM");
            await Test(new DateTime(2020, 1, 1, 14, 35, 55), "m/d/yyyy\\ h:mm:ss A/P;@", "1/1/2020 2:35:55 P");
            await Test(new DateTime(2020, 1, 1, 14, 35, 55), "m/d/yyyy\\ h:mm:ss a/P;@", "1/1/2020 2:35:55 p");
            await Test(new DateTime(2020, 1, 1, 14, 35, 55), "m/d/yyyy\\ h:mm:ss A/p;@", "1/1/2020 2:35:55 P");
            await Test(new DateTime(2020, 1, 1, 14, 35, 55), "m/d/yyyy\\ h:mm:ss;@", "1/1/2020 14:35:55");
            await Test(new DateTime(2020, 1, 1, 14, 35, 55), "m/d/yyyy\\ hh:mm:ss AM/PM;@", "1/1/2020 02:35:55 PM");
            await Test(new DateTime(2020, 1, 1, 16, 5, 6), "m/d/yyyy\\ h:m:s AM/PM;@", "1/1/2020 4:5:6 PM");
            await Test(new DateTime(2017, 10, 16, 0, 0, 0), "dddd, MMMM d, yyyy", "Monday, October 16, 2017");
            await Test(new DateTime(2017, 10, 16, 0, 0, 0), "dddd,,, MMMM d,, yyyy,,,,", "Monday, October 16, 2017,");
            await Test(new DateTime(2020, 1, 1, 0, 35, 55), "m/d/yyyy\\ hh:mm:ss AM/PM;@", "1/1/2020 12:35:55 AM");
            await Test(new DateTime(2020, 1, 1, 12, 35, 55), "m/d/yyyy\\ hh:mm:ss AM/PM;@", "1/1/2020 12:35:55 PM");
        }

        [Test]
        public async Task TestNumericDate1900()
        {
            await Test("0", "dd/mm/yyyy", "0");
            await Test(0, "dd/mm/yyyy", "00/01/1900");
            await Test(0d, "dd/mm/yyyy", "00/01/1900");
            await Test((short)0, "dd/mm/yyyy", "00/01/1900");
            await Test(1, "dd/mm/yyyy", "01/01/1900");
            await Test(60, "dd/mm/yyyy", "29/02/1900");
            await Test(61, "dd/mm/yyyy", "01/03/1900");
            await Test(43648, "[$-409]d\\-mmm\\-yyyy;@", "2-Jul-2019");
        }

        [Test]
        public async Task TestNumericDate1904()
        {
            await Test(0, "dd/mm/yyyy", "01/01/1904", true);
            await Test(0d, "dd/mm/yyyy", "01/01/1904", true);
            await Test((short)0, "dd/mm/yyyy", "01/01/1904", true);
            await Test(1, "dd/mm/yyyy", "02/01/1904", true);
            await Test(60, "dd/mm/yyyy", "01/03/1904", true);
            await Test(61, "dd/mm/yyyy", "02/03/1904", true);
        }

        [Test]
        public async Task TestNumericDuration()
        {
            await Test(0, "[hh]:mm", "00:00");
            await Test(1, "[hh]:mm", "24:00");
            await Test(1.5, "[hh]:mm", "36:00");
        }

        [Test]
        public async Task TestTimeSpan()
        {
            await Test(TimeSpan.FromHours(100), "[hh]:mm:ss", "100:00:00");
            await Test(TimeSpan.FromHours(100), "[mm]:ss", "6000:00");
            await Test(TimeSpan.FromMilliseconds(100 * 60 * 60 * 1000 + 123), "[mm]:ss.000", "6000:00.123");

            await Test(new TimeSpan(1, 2, 31, 45), "[hh]:mm:ss", "26:31:45");
            await Test(new TimeSpan(1, 2, 31, 44, 500), "[hh]:mm:ss", "26:31:45");
            await Test(new TimeSpan(1, 2, 31, 44, 500), "[hh]:mm:ss.000", "26:31:44.500");

            await Test(new TimeSpan(-1, -2, -31, -45), "[hh]:mm:ss", "-26:31:45");
            await Test(new TimeSpan(0, -2, -31, -45), "[hh]:mm:ss", "-02:31:45");
            await Test(new TimeSpan(0, -2, -31, -44, -500), "[hh]:mm:ss", "-02:31:45");
            await Test(new TimeSpan(0, -2, -31, -44, -500), "[hh]:mm:ss.000", "-02:31:44.500");
        }

        [Test]
        public async Task TestSystemLongDateTime()
        {
            var date = 45498.0; // 2024-07-25
            var dateTime = 45498.5;
            var fr = new CultureInfo("fr-FR");
            var us = new CultureInfo("en-US");

            await Assert.That(Format(date, @"[$-F800]dddd\,\ mmmm\ dd\,\ yyyy", fr)).IsEqualTo(new DateTime(2024, 7, 25).ToString(fr.DateTimeFormat.LongDatePattern, fr));
            await Assert.That(Format(date, @"[$-F800]dddd\,\ mmmm\ dd\,\ yyyy", us)).IsEqualTo("Thursday, July 25, 2024");
            await Assert.That(Format(dateTime, "[$-F400]h:mm:ss AM/PM", fr)).IsEqualTo(new DateTime(2024, 7, 25, 12, 0, 0).ToString(fr.DateTimeFormat.LongTimePattern, fr));
            await Assert.That(Format(dateTime, "[$-F400]h:mm:ss AM/PM", us)).IsEqualTo(new DateTime(2024, 7, 25, 12, 0, 0).ToString(us.DateTimeFormat.LongTimePattern, us));

            // Other locale ids still use the tokens that follow
            await Assert.That(Format(date, "[$-409]yyyy-mm-dd", fr)).IsEqualTo("2024-07-25");
        }

        [Test]
        public async Task TestDateOnlyAndTimeOnly()
        {
            await Test(new DateOnly(2024, 7, 25), "yyyy-mm-dd", "2024-07-25");
            await Test(new TimeOnly(13, 45, 30), "hh:mm:ss", "13:45:30");
            await Test(new TimeOnly(13, 45, 30), "h:mm AM/PM", "1:45 PM");
        }

        [Test]
        [Arguments("da-DK", "17.08.1978")]
        [Arguments("en-US", "17/08/1978")]
        [Arguments("bg-BG", "17.08.1978")]
        [Arguments("nb-NO", "17.08.1978")]
        public async Task TestDateSeparatorCulture(string cultureName, string expected)
        {
            var actual = Format(new DateTime(1978, 8, 17), "DD/MM/YYYY", new CultureInfo(cultureName));
            await Assert.That(actual).IsEqualTo(expected);
        }
    }
}
