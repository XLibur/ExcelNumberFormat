using System;
using System.Globalization;
using System.Threading.Tasks;
using static ExcelNumberFormat.Tests.FormatHelper;

// Much of the test data was adapted from the SheetJS/ssf project.
// Copyright (C) 2013-present  SheetJS. 
// Licensed under the Apache License, Version 2.0

namespace ExcelNumberFormat.Tests
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
        public async Task TestFractionalSeconds()
        {
            // Expected values were checked against Excel 16, which shows as many digits as there are zeros and rounds them
            await Test(12.34 / 86400, "mm:ss.0", "00:12.3");
            await Test(12.34 / 86400, "mm:ss.00", "00:12.34");
            await Test(12.345 / 86400, "mm:ss.00", "00:12.35");
            await Test(12.35 / 86400, "mm:ss.0", "00:12.4");
            await Test(588.34 / 86400, "mm:ss.0", "09:48.3");
            await Test(12.3456 / 86400, "mm:ss.000", "00:12.346");
            await Test(0.5 / 86400, "mm:ss.0", "00:00.5");

            // Stored just below .25, so Excel rounds down
            await Test(3661.25 / 86400, "h:mm:ss.0", "1:01:01.2");
            await Test(3661.26 / 86400, "h:mm:ss.0", "1:01:01.3");
            await Test(3661.26 / 86400, "h:mm:ss.00", "1:01:01.26");

            // Rounding carries into seconds, minutes and hours
            await Test(59.96 / 86400, "mm:ss.0", "01:00.0");
            await Test(59.996 / 86400, "mm:ss.00", "01:00.00");
            await Test(59.96 / 86400, "ss.0", "00.0");
            await Test(3599.96 / 86400, "mm:ss.0", "00:00.0");

            await Test(45498.123456, "mm:ss.00", "57:46.60");
            await Test(45498.123456, "yyyy-mm-dd hh:mm:ss.0", "2024-07-25 02:57:46.6");
            await Test(new DateTime(2024, 7, 25, 2, 57, 46, 598), "hh:mm:ss.0", "02:57:46.6");
            await Test(new DateTime(2024, 7, 25, 2, 57, 59, 960), "hh:mm:ss.0", "02:58:00.0");
        }

        [Test]
        public async Task TestFractionalSecondsDuration()
        {
            await Test(1.96 / 86400, "[ss].0", "02.0");
            await Test(125.76 / 86400, "[mm]:ss.0", "02:05.8");
            await Test(125.76 / 86400, "[h]:mm:ss.0", "0:02:05.8");
            await Test(new TimeSpan(0, 0, 2, 5, 760), "[mm]:ss.00", "02:05.76");
            await Test(new TimeSpan(0, 0, 2, 5, 760), "[mm]:ss.0", "02:05.8");
            await Test(new TimeSpan(0, 0, 0, 59, 960), "[mm]:ss.0", "01:00.0");
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

        [Test]
        public async Task TestOutOfRangeDateSerials()
        {
            // No date for these, so the value is shown as it is rather than throwing
            await Test(1e12, "yyyy-mm-dd", "1000000000000");
            await Test(double.NaN, "yyyy-mm-dd", "NaN");
            await Test(double.PositiveInfinity, "yyyy-mm-dd hh:mm", "Infinity");
            await Test(-1e12, "yyyy-mm-dd", "-1000000000000");
        }

        [Test]
        public async Task TestLargeDurations()
        {
            // More than 2^31 seconds does not wrap around
            await Test(new TimeSpan(30000, 0, 0, 0), "[s]", "2592000000");
            await Test(30000.5, "[s]", "2592043200");
        }
    }
}
