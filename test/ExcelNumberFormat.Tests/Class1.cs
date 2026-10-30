using System;
using System.Globalization;
using System.Threading.Tasks;

// Much of the test data was adapted from the SheetJS/ssf project.
// Copyright (C) 2013-present  SheetJS. 
// Licensed under the Apache License, Version 2.0

namespace Xlibur.ExcelNumberFormat.Tests
{
    public class Class1
    {

        string Format(object value, string formatString, CultureInfo culture, bool isDate1904 = false)
        {
            var format = new NumberFormat(formatString);
            if (format.IsValid)
                return format.Format(value, culture, isDate1904);

            return null;
        }

        bool IsDateFormatString(string formatString)
        {
            var format = new NumberFormat(formatString);
            return format?.IsDateTimeFormat ?? false;
        }

        [Test]
        public async Task TestCondition()
        {
            await Test("Hello", "\"p\"0;\"m\"0;\"z\"0;\"t\"@", "tHello");
            await Test("Hello", "\"num\"0", "Hello");

            await Test(-1, "\"p\"0;\"m\"0;\"z\"0;\"t\"@", "m1");
            await Test(0, "\"p\"0;\"m\"0;\"z\"0;\"t\"@", "z0");
            await Test(1, "\"p\"0;\"m\"0;\"z\"0;\"t\"@", "p1");

            await Test(-1, "[<0]\"p\"0;\"m\"0;\"z\"0;\"t\"@", "p1");
            await Test(0, "[<0]\"p\"0;\"m\"0;\"z\"0;\"t\"@", "z0");
            await Test(1, "[<0]\"p\"0;\"m\"0;\"z\"0;\"t\"@", "z1");

            await Test(-1, "\"p\"0;[>0]\"m\"0;\"z\"0;\"t\"@", "-z1");
            await Test(0, "\"p\"0;[>0]\"m\"0;\"z\"0;\"t\"@", "z0");
            await Test(1, "\"p\"0;[>0]\"m\"0;\"z\"0;\"t\"@", "p1");

            await Test(-1, "[<0]\"LT0\";\"ELSE\"", "LT0");
            await Test(0, "[<0]\"LT0\";\"ELSE\"", "ELSE");
            await Test(1, "[<0]\"LT0\";\"ELSE\"", "ELSE");

            await Test(-1, "[<=0]\"LTE0\";\"ELSE\"", "LTE0");
            await Test(0, "[<=0]\"LTE0\";\"ELSE\"", "LTE0");
            await Test(1, "[<=0]\"LTE0\";\"ELSE\"", "ELSE");

            await Test(-1, "[>0]\"GT0\";\"ELSE\"", "ELSE");
            await Test(0, "[>0]\"GT0\";\"ELSE\"", "ELSE");
            await Test(1, "[>0]\"GT0\";\"ELSE\"", "GT0");

            await Test(-1, "[>=0]\"GTE0\";\"ELSE\"", "ELSE");
            await Test(0, "[>=0]\"GTE0\";\"ELSE\"", "GTE0");
            await Test(1, "[>=0]\"GTE0\";\"ELSE\"", "GTE0");

            await Test(-1, "[=0]\"EQ0\";\"ELSE\"", "ELSE");
            await Test(0, "[=0]\"EQ0\";\"ELSE\"", "EQ0");
            await Test(1, "[=0]\"EQ0\";\"ELSE\"", "ELSE");

            await Test(-1, "[<>0]\"NEQ0\";\"ELSE\"", "NEQ0");
            await Test(0, "[<>0]\"NEQ0\";\"ELSE\"", "ELSE");
            await Test(1, "[<>0]\"NEQ0\";\"ELSE\"", "NEQ0");
        }

        [Test]
        public async Task TestFractionAlignmentSuffix()
        {
            await Test(0,   "??/??", " 0/1 ");
            await Test(1.5, "??/??", " 3/2 ");
            await Test(3.4, "??/??", "17/5 ");
            await Test(4.3, "??/??", "43/10");

            await Test(0,   "00/00", "00/01");
            await Test(1.5, "00/00", "03/02");
            await Test(3.4, "00/00", "17/05");
            await Test(4.3, "00/00", "43/10");

            await Test(0.00, "# ??/\"a\"?\"a\"0\"a\"", "0        a");
            await Test(0.10, "# ??/\"a\"?\"a\"0\"a\"", "0        a");
            await Test(0.12, "# ??/\"a\"?\"a\"0\"a\"", "  1/a8a0a");

            await Test(1.00, "# ??/\"a\"?\"a\"0\"a\"", "1        a");
            await Test(1.10, "# ??/\"a\"?\"a\"0\"a\"", "1  1/a9a0a");
            await Test(1.12, "# ??/\"a\"?\"a\"0\"a\"", "1  1/a8a0a");
        }


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

        async Task Test(object value, string format, string expected, bool isDate1904 = false)
        {
            var result = Format(value, format, CultureInfo.InvariantCulture, isDate1904);
            await Assert.That(result).IsEqualTo(expected);
        }

        [Test]
        public async Task TestFraction()
        {
            await Test(1, "# ?/?", "1    ");
            await Test(-1.2, "# ?/?", "-1 1/5");
            await Test(12.3, "# ?/?", "12 1/3");
            await Test(-12.34, "# ?/?", "-12 1/3");
            await Test(123.45, "# ?/?", "123 4/9");
            await Test(-123.456, "# ?/?", "-123 1/2");
            await Test(1234.567, "# ?/?", "1234 4/7");
            await Test(-1234.5678, "# ?/?", "-1234 4/7");
            await Test(12345.6789, "# ?/?", "12345 2/3");
            await Test(-12345.67891, "# ?/?", "-12345 2/3");

            await Test(1, "# ??/??", "1      ");
            await Test(-1.2, "# ??/??", "-1  1/5 ");
            await Test(12.3, "# ??/??", "12  3/10");
            await Test(-12.34, "# ??/??", "-12 17/50");
            await Test(123.45, "# ??/??", "123  9/20");
            await Test(-123.456, "# ??/??", "-123 26/57");
            await Test(1234.567, "# ??/??", "1234 55/97");
            await Test(-1234.5678, "# ??/??", "-1234 46/81");
            await Test(12345.6789, "# ??/??", "12345 55/81");
            await Test(-12345.67891, "# ??/??", "-12345 55/81");

            await Test(1, "# ???/???", "1        ");
            await Test(-1.2, "# ???/???", "-1   1/5  ");
            await Test(12.3, "# ???/???", "12   3/10 ");
            await Test(-12.34, "# ???/???", "-12  17/50 ");
            await Test(123.45, "# ???/???", "123   9/20 ");
            await Test(-123.456, "# ???/???", "-123  57/125");
            await Test(1234.567, "# ???/???", "1234  55/97 ");
            await Test(-1234.5678, "# ???/???", "-1234  67/118");
            await Test(12345.6789, "# ???/???", "12345  74/109");
            await Test(-12345.67891, "# ???/???", "-12345 573/844");


            await Test(1, "# ?/2", "1    ");
            await Test(-1.2, "# ?/2", "-1    ");
            await Test(12.3, "# ?/2", "12 1/2");
            await Test(-12.34, "# ?/2", "-12 1/2");
            await Test(123.45, "# ?/2", "123 1/2");
            await Test(-123.456, "# ?/2", "-123 1/2");
            await Test(1234.567, "# ?/2", "1234 1/2");
            await Test(-1234.5678, "# ?/2", "-1234 1/2");
            await Test(12345.6789, "# ?/2", "12345 1/2");
            await Test(-12345.67891, "# ?/2", "-12345 1/2");

            await Test(1, "# ?/4", "1    ");
            await Test(-1.2, "# ?/4", "-1 1/4");
            await Test(12.3, "# ?/4", "12 1/4");
            await Test(-12.34, "# ?/4", "-12 1/4");
            await Test(123.45, "# ?/4", "123 2/4");
            await Test(-123.456, "# ?/4", "-123 2/4");
            await Test(1234.567, "# ?/4", "1234 2/4");
            await Test(-1234.5678, "# ?/4", "-1234 2/4");
            await Test(12345.6789, "# ?/4", "12345 3/4");
            await Test(-12345.67891, "# ?/4", "-12345 3/4");

            await Test(1, "# ?/8", "1    ");
            await Test(-1.2, "# ?/8", "-1 2/8");
            await Test(12.3, "# ?/8", "12 2/8");
            await Test(-12.34, "# ?/8", "-12 3/8");
            await Test(123.45, "# ?/8", "123 4/8");
            await Test(-123.456, "# ?/8", "-123 4/8");
            await Test(1234.567, "# ?/8", "1234 5/8");
            await Test(-1234.5678, "# ?/8", "-1234 5/8");
            await Test(12345.6789, "# ?/8", "12345 5/8");
            await Test(-12345.67891, "# ?/8", "-12345 5/8");

            await Test(1, "# ??/16", "1      ");
            await Test(-1.2, "# ??/16", "-1  3/16");
            await Test(12.3, "# ??/16", "12  5/16");
            await Test(-12.34, "# ??/16", "-12  5/16");
            await Test(123.45, "# ??/16", "123  7/16");
            await Test(-123.456, "# ??/16", "-123  7/16");
            await Test(1234.567, "# ??/16", "1234  9/16");
            await Test(-1234.5678, "# ??/16", "-1234  9/16");
            await Test(12345.6789, "# ??/16", "12345 11/16");
            await Test(-12345.67891, "# ??/16", "-12345 11/16");

            await Test(1, "# ?/10", "1     ");
            await Test(-1.2, "# ?/10", "-1 2/10");
            await Test(12.3, "# ?/10", "12 3/10");
            await Test(-12.34, "# ?/10", "-12 3/10");
            await Test(123.45, "# ?/10", "123 5/10");
            await Test(-123.456, "# ?/10", "-123 5/10");
            await Test(1234.567, "# ?/10", "1234 6/10");
            await Test(-1234.5678, "# ?/10", "-1234 6/10");
            await Test(12345.6789, "# ?/10", "12345 7/10");
            await Test(-12345.67891, "# ?/10", "-12345 7/10");

            await Test(1, "# ??/100", "1       ");
            await Test(-1.2, "# ??/100", "-1 20/100");
            await Test(12.3, "# ??/100", "12 30/100");
            await Test(-12.34, "# ??/100", "-12 34/100");
            await Test(123.45, "# ??/100", "123 45/100");
            await Test(-123.456, "# ??/100", "-123 46/100");
            await Test(1234.567, "# ??/100", "1234 57/100");
            await Test(-1234.5678, "# ??/100", "-1234 57/100");
            await Test(12345.6789, "# ??/100", "12345 68/100");
            await Test(-12345.67891, "# ??/100", "-12345 68/100");

            await Test(1, "??/??", " 1/1 ");
            await Test(-1.2, "??/??", "- 6/5 ");
            await Test(12.3, "??/??", "123/10");
            await Test(-12.34, "??/??", "-617/50");
            await Test(123.45, "??/??", "2469/20");
            await Test(-123.456, "??/??", "-7037/57");
            await Test(1234.567, "??/??", "119753/97");
            await Test(-1234.5678, "??/??", "-100000/81");
            await Test(12345.6789, "??/??", "1000000/81");
            await Test(-12345.67891, "??/??", "-1000000/81");

            await Test(0.3, "# ?/?", " 2/7");
            await Test(1.3, "# ?/?", "1 1/3");
            await Test(2.3, "# ?/?", "2 2/7");

            // Not sure what/why ssf does here:
            // Test(0.123251512342345, "# ??/?????????", "  480894/3901729");
            // Test(0.123251512342345, "# ?? / ?????????", "  480894 / 3901729");
            // This implementation instead renders like this:
            await Test(0.123251512342345, "# ??/?????????", " 480894/3901729  ");
            await Test(0.123251512342345, "# ?? / ?????????", " 480894 / 3901729  ");

            await Test(0, "0", "0");

        }

        async Task TestExponents(double value, string expected1, string expected2, string expected3, string expected4)
        {
            // value	#0.0E+0	##0.0E+0	###0.0E+0	####0.0E+0
            var result1 = Format(value, "#0.0E+0", CultureInfo.InvariantCulture);
            await Assert.That(result1).IsEqualTo(expected1);

            var result2 = Format(value, "##0.0E+0", CultureInfo.InvariantCulture);
            await Assert.That(result2).IsEqualTo(expected2);

            var result3 = Format(value, "###0.0E+0", CultureInfo.InvariantCulture);
            await Assert.That(result3).IsEqualTo(expected3);

            var result4 = Format(value, "####0.0E+0", CultureInfo.InvariantCulture);
            await Assert.That(result4).IsEqualTo(expected4);
        }

        async Task TestNumber(double value, string expected1, string expected2, string expected3, string expected4, string expected5)
        {
            // value	?.?	??.??	???.???	???.?0?	???.?#?
            var result1 = Format(value, "?.?", CultureInfo.InvariantCulture);
            await Assert.That(result1).IsEqualTo(expected1);

            var result2 = Format(value, "??.??", CultureInfo.InvariantCulture);
            await Assert.That(result2).IsEqualTo(expected2);

            var result3 = Format(value, "???.???", CultureInfo.InvariantCulture);
            await Assert.That(result3).IsEqualTo(expected3);

            var result4 = Format(value, "???.?0?", CultureInfo.InvariantCulture);
            await Assert.That(result4).IsEqualTo(expected4);

            var result5 = Format(value, "???.?#?", CultureInfo.InvariantCulture);
            await Assert.That(result5).IsEqualTo(expected5);
        }


        [Test]
        public async Task TestNumber()
        {
            await TestNumber(  0.0, " . ", "  .  ", "   .   ", "   . 0 ", "   .  ");
            await TestNumber(  0.1, " .1", "  .1 ", "   .1  ", "   .10 ", "   .1 ");
            await TestNumber( 0.12, " .1", "  .12", "   .12 ", "   .12 ", "   .12 ");
            await TestNumber(0.123, " .1", "  .12", "   .123", "   .123", "   .123");

            await TestNumber(  1.0, "1. ", " 1.  ", "  1.   ", "  1. 0 ", "  1.  ");
            await TestNumber(  1.1, "1.1", " 1.1 ", "  1.1  ", "  1.10 ", "  1.1 ");
            await TestNumber( 1.12, "1.1", " 1.12", "  1.12 ", "  1.12 ", "  1.12 ");
            await TestNumber(1.123, "1.1", " 1.12", "  1.123", "  1.123", "  1.123");
        }

        [Test]
        public async Task TestExponent()
        {
            await TestExponents(-1.23457E-13, "-12.3E-14", "-123.5E-15", "-1234.6E-16", "-123.5E-15");
            await TestExponents(-12345.6789, "-1.2E+4", "-12.3E+3", "-1.2E+4", "-12345.7E+0");

            await TestExponents(1.23457E-13, "12.3E-14", "123.5E-15", "1234.6E-16", "123.5E-15");
            await TestExponents(1.23457E-12, "1.2E-12", "1.2E-12", "1.2E-12", "1234.6E-15");
            await TestExponents(1.23457E-11, "12.3E-12", "12.3E-12", "12.3E-12", "12345.7E-15");
            await TestExponents(1.23457E-10, "1.2E-10", "123.5E-12", "123.5E-12", "1.2E-10");
            await TestExponents(1.23457E-09, "12.3E-10", "1.2E-9", "1234.6E-12", "12.3E-10");
            await TestExponents(1.23457E-08, "1.2E-8", "12.3E-9", "1.2E-8", "123.5E-10");
            await TestExponents(0.000000123457, "12.3E-8", "123.5E-9", "12.3E-8", "1234.6E-10");
            await TestExponents(0.00000123457, "1.2E-6", "1.2E-6", "123.5E-8", "12345.7E-10");
            await TestExponents(0.0000123457, "12.3E-6", "12.3E-6", "1234.6E-8", "1.2E-5");
            await TestExponents(0.000123457, "1.2E-4", "123.5E-6", "1.2E-4", "12.3E-5");
            await TestExponents(0.001234568, "12.3E-4", "1.2E-3", "12.3E-4", "123.5E-5");
            await TestExponents(0.012345679, "1.2E-2", "12.3E-3", "123.5E-4", "1234.6E-5");
            await TestExponents(0.123456789, "12.3E-2", "123.5E-3", "1234.6E-4", "12345.7E-5");
            await TestExponents(1.23456789, "1.2E+0", "1.2E+0", "1.2E+0", "1.2E+0");
            await TestExponents(12.3456789, "12.3E+0", "12.3E+0", "12.3E+0", "12.3E+0");
            await TestExponents(123.456789, "1.2E+2", "123.5E+0", "123.5E+0", "123.5E+0");
            await TestExponents(1234.56789, "12.3E+2", "1.2E+3", "1234.6E+0", "1234.6E+0");
            await TestExponents(12345.6789, "1.2E+4", "12.3E+3", "1.2E+4", "12345.7E+0");
            await TestExponents(123456.789, "12.3E+4", "123.5E+3", "12.3E+4", "1.2E+5");
            await TestExponents(1234567.89, "1.2E+6", "1.2E+6", "123.5E+4", "12.3E+5");
            await TestExponents(12345678.9, "12.3E+6", "12.3E+6", "1234.6E+4", "123.5E+5");
            await TestExponents(123456789D, "1.2E+8", "123.5E+6", "1.2E+8", "1234.6E+5");
            await TestExponents(1234567890D, "12.3E+8", "1.2E+9", "12.3E+8", "12345.7E+5");
            await TestExponents(12345678900D, "1.2E+10", "12.3E+9", "123.5E+8", "1.2E+10");
            await TestExponents(123456789000D, "12.3E+10", "123.5E+9", "1234.6E+8", "12.3E+10");
            await TestExponents(1234567890000D, "1.2E+12", "1.2E+12", "1.2E+12", "123.5E+10");
            await TestExponents(12345678900000D, "12.3E+12", "12.3E+12", "12.3E+12", "1234.6E+10");
            await TestExponents(123456789000000D, "1.2E+14", "123.5E+12", "123.5E+12", "12345.7E+10");
            await TestExponents(1234567890000000D, "12.3E+14", "1.2E+15", "1234.6E+12", "1.2E+15");
            await TestExponents(12345678900000000D, "1.2E+16", "12.3E+15", "1.2E+16", "12.3E+15");
            await TestExponents(123456789000000000D, "12.3E+16", "123.5E+15", "12.3E+16", "123.5E+15");
            await TestExponents(1234567890000000000D, "1.2E+18", "1.2E+18", "123.5E+16", "1234.6E+15");
            await TestExponents(12345678900000000000D, "12.3E+18", "12.3E+18", "1234.6E+16", "12345.7E+15");
            await TestExponents(123456789000000000000D, "1.2E+20", "123.5E+18", "1.2E+20", "1.2E+20");
            await TestExponents(1234567890000000000000D, "12.3E+20", "1.2E+21", "12.3E+20", "12.3E+20");
            await TestExponents(12345678900000000000000D, "1.2E+22", "12.3E+21", "123.5E+20", "123.5E+20");
            await TestExponents(123456789000000000000000D, "12.3E+22", "123.5E+21", "1234.6E+20", "1234.6E+20");
            await TestExponents(1234567890000000000000000D, "1.2E+24", "1.2E+24", "1.2E+24", "12345.7E+20");
            await TestExponents(12345678900000000000000000D, "12.3E+24", "12.3E+24", "12.3E+24", "1.2E+25");
            await TestExponents(123456789000000000000000000D, "1.2E+26", "123.5E+24", "123.5E+24", "12.3E+25");
            await TestExponents(1234567890000000000000000000D, "12.3E+26", "1.2E+27", "1234.6E+24", "123.5E+25");
            await TestExponents(12345678900000000000000000000D, "1.2E+28", "12.3E+27", "1.2E+28", "1234.6E+25");
            await TestExponents(123456789000000000000000000000D, "12.3E+28", "123.5E+27", "12.3E+28", "12345.7E+25");
            await TestExponents(1234567890000000000000000000000D, "1.2E+30", "1.2E+30", "123.5E+28", "1.2E+30");
            await TestExponents(12345678900000000000000000000000D, "12.3E+30", "12.3E+30", "1234.6E+28", "12.3E+30");
        }

        async Task TestComma(double value, string expected1, string expected2, string expected3, string expected4, string expected5, string expected6, string expected7)
        {
            // value	#.0000,,,	#.0000,,	#.0000,	#,##0.0	###,##0	###,###	#,###.00
            var result1 = Format(value, "#.0000,,,", CultureInfo.InvariantCulture);
            await Assert.That(result1).IsEqualTo(expected1);

            var result2 = Format(value, "#.0000,,", CultureInfo.InvariantCulture);
            await Assert.That(result2).IsEqualTo(expected2);

            var result3 = Format(value, "#.0000,", CultureInfo.InvariantCulture);
            await Assert.That(result3).IsEqualTo(expected3);

            var result4 = Format(value, "#,##0.0", CultureInfo.InvariantCulture);
            await Assert.That(result4).IsEqualTo(expected4);

            var result5 = Format(value, "###,##0", CultureInfo.InvariantCulture);
            await Assert.That(result5).IsEqualTo(expected5);

            var result6 = Format(value, "###,###", CultureInfo.InvariantCulture);
            await Assert.That(result6).IsEqualTo(expected6);

            var result7 = Format(value, "#,###.00", CultureInfo.InvariantCulture);
            await Assert.That(result7).IsEqualTo(expected7);
        }

        [Test]
        public async Task TestComma()
        {
            await TestComma(0.99, ".0000", ".0000", ".0010", "1.0", "1", "1", ".99");
            await TestComma(1.2345, ".0000", ".0000", ".0012", "1.2", "1", "1", "1.23");
            await TestComma(12.345, ".0000", ".0000", ".0123", "12.3", "12", "12", "12.35");
            await TestComma(123.456, ".0000", ".0001", ".1235", "123.5", "123", "123", "123.46");
            await TestComma(1234, ".0000", ".0012", "1.2340", "1,234.0", "1,234", "1,234", "1,234.00");
            await TestComma(12345, ".0000", ".0123", "12.3450", "12,345.0", "12,345", "12,345", "12,345.00");
            await TestComma(123456, ".0001", ".1235", "123.4560", "123,456.0", "123,456", "123,456", "123,456.00");
            await TestComma(1234567, ".0012", "1.2346", "1234.5670", "1,234,567.0", "1,234,567", "1,234,567", "1,234,567.00");
            await TestComma(12345678, ".0123", "12.3457", "12345.6780", "12,345,678.0", "12,345,678", "12,345,678", "12,345,678.00");
            await TestComma(123456789, ".1235", "123.4568", "123456.7890", "123,456,789.0", "123,456,789", "123,456,789", "123,456,789.00");
            await TestComma(1234567890, "1.2346", "1234.5679", "1234567.8900", "1,234,567,890.0", "1,234,567,890", "1,234,567,890", "1,234,567,890.00");
            await TestComma(12345678901, "12.3457", "12345.6789", "12345678.9010", "12,345,678,901.0", "12,345,678,901", "12,345,678,901", "12,345,678,901.00");
            await TestComma(123456789012, "123.4568", "123456.7890", "123456789.0120", "123,456,789,012.0", "123,456,789,012", "123,456,789,012", "123,456,789,012.00");
            await TestComma(4321, ".0000", ".0043", "4.3210", "4,321.0", "4,321", "4,321", "4,321.00");
            await TestComma(4321234, ".0043", "4.3212", "4321.2340", "4,321,234.0", "4,321,234", "4,321,234", "4,321,234.00");

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
        public async Task TestRoundHalfAwayFromZero()
        {
            await Test(2.5, "0", "3");
            await Test(3.5, "0", "4");
            await Test(-2.5, "0", "-3");
            await Test(1234.5, "#,##0", "1,235");
            await Test(0.125, "0.00", "0.13");
            await Test(2.675, "0.00", "2.68");
            await Test(0.125, "0.0%", "12.5%");
            await Test(0.0125, "0.0%", "1.3%");
            await Test(1.5, "0", "2");
            await Test(0.4, "0", "0");
        }

        [Test]
        public async Task TestTextSectionLiterals()
        {
            await Test("abc", "\"Prefix: \"@", "Prefix: abc");
            await Test("abc", "\"x\"@\"y\"", "xabcy");
            await Test("abc", "0;-0;0;\"Note: \"@", "Note: abc");
            await Test("abc", "0;\"Text: \"@", "Text: abc");
            await Test("abc", "0;-0;\"Text: \"@", "Text: abc");
            await Test("abc", "@", "abc");
            await Test("abc", "0;-0;0", "abc");
            await Test("abc", "0.00", "abc");
        }

        [Test]
        public async Task TestThousandSeparator()
        {
            var actual = Format(1469.07, "0,000,000.00", CultureInfo.InvariantCulture);
            await Assert.That(actual).IsEqualTo("0,001,469.07");
        }

        [Test]
        public async Task TestThousandSeparatorCulture()
        {
            var actual = Format(1469.07, "0,000,000.00", new CultureInfo("da-DK"));
            await Assert.That(actual).IsEqualTo("0.001.469,07");
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

        async Task TestValid(string format)
        {
            var to = new NumberFormat(format);
            await Assert.That(to.IsValid).IsTrue();
        }

        [Test]
        public async Task TestValid()
        {
            await TestValid("\" Excellent\"");
            await TestValid("\" Fair\"");
            await TestValid("\" Good\"");
            await TestValid("\" Poor\"");
            await TestValid("\" Very Good\"");
            await TestValid("\"$\"#,##0");
            await TestValid("\"$\"#,##0.00");
            await TestValid("\"$\"#,##0.00_);[Red]\\(\"$\"#,##0.00\\)");
            await TestValid("\"$\"#,##0.00_);\\(\"$\"#,##0.00\\)");
            await TestValid("\"$\"#,##0;[Red]\\-\"$\"#,##0");
            await TestValid("\"$\"#,##0_);[Red]\\(\"$\"#,##0\\)");
            await TestValid("\"$\"#,##0_);\\(\"$\"#,##0\\)");
            await TestValid("\"Haha!\"\\ @\\ \"Yeah!\"");
            await TestValid("\"TRUE\";\"TRUE\";\"FALSE\"");
            await TestValid("\"True\";\"True\";\"False\";@");
            await TestValid("\"Years: \"0");
            await TestValid("\"Yes\";\"Yes\";\"No\";@");
            await TestValid("\"kl \"hh:mm:ss;@");
            await TestValid("\"£\"#,##0.00");
            await TestValid("\"£\"#,##0;[Red]\\-\"£\"#,##0");
            await TestValid("\"€\"#,##0.00");
            await TestValid("\"€\"\\ #,##0.00_-");
            await TestValid("\"上午/下午 \"hh\"時\"mm\"分\"ss\"秒 \"");
            await TestValid("\"￥\"#,##0.00;\"￥\"\\-#,##0.00");
            await TestValid("#");
            await TestValid("# ?/?");
            await TestValid("# ??/??");
            await TestValid("#\" \"?/?");
            await TestValid("#\" \"??/??");
            await TestValid("#\"abded\"\\ ??/??");
            await TestValid("###0.00;-###0.00");
            await TestValid("###0;-###0");
            await TestValid("##0.0E+0");
            await TestValid("#,##0");
            await TestValid("#,##0 ;(#,##0)");
            await TestValid("#,##0 ;[Red](#,##0)");
            await TestValid("#,##0\"р.\";[Red]\\-#,##0\"р.\"");
            await TestValid("#,##0.0");
            await TestValid("#,##0.00");
            await TestValid("#,##0.00 \"�\"");
            await TestValid("#,##0.00 €;-#,##0.00 €");
            await TestValid("#,##0.00\"р.\";[Red]\\-#,##0.00\"р.\"");
            await TestValid("#,##0.000");
            await TestValid("#,##0.0000");
            await TestValid("#,##0.00000");
            await TestValid("#,##0.000000");
            await TestValid("#,##0.0000000");
            await TestValid("#,##0.00000000");
            await TestValid("#,##0.000000000");
            await TestValid("#,##0.00000000;[Red]#,##0.00000000");
            await TestValid("#,##0.0000_ ");
            await TestValid("#,##0.000_ ");
            await TestValid("#,##0.000_);\\(#,##0.000\\)");
            await TestValid("#,##0.00;(#,##0.00)");
            await TestValid("#,##0.00;(#,##0.00);0.00");
            await TestValid("#,##0.00;[Red](#,##0.00)");
            await TestValid("#,##0.00;[Red]\\(#,##0.00\\)");
            await TestValid("#,##0.00;\\(#,##0.00\\)");
            await TestValid("#,##0.00[$₹-449]_);\\(#,##0.00[$₹-449]\\)");
            await TestValid("#,##0.00\\ \"р.\"");
            await TestValid("#,##0.00\\ \"р.\";[Red]\\-#,##0.00\\ \"р.\"");
            await TestValid("#,##0.00\\ [$€-407]");
            await TestValid("#,##0.00\\ [$€-40C]");
            await TestValid("#,##0.00_);\\(#,##0.00\\)");
            await TestValid("#,##0.00_р_.;[Red]\\-#,##0.00_р_.");
            await TestValid("#,##0.00_р_.;\\-#,##0.00_р_.");
            await TestValid("#,##0.0;[Red]#,##0.0");
            await TestValid("#,##0.0_ ;\\-#,##0.0\\ ");
            await TestValid("#,##0.0_);[Red]\\(#,##0.0\\)");
            await TestValid("#,##0.0_);\\(#,##0.0\\)");
            await TestValid("#,##0;\\-#,##0;0");
            await TestValid("#,##0\\ \"р.\";[Red]\\-#,##0\\ \"р.\"");
            await TestValid("#,##0\\ \"р.\";\\-#,##0\\ \"р.\"");
            await TestValid("#,##0\\ ;[Red]\\(#,##0\\)");
            await TestValid("#,##0\\ ;\\(#,##0\\)");
            await TestValid("#,##0_ ");
            await TestValid("#,##0_ ;[Red]\\-#,##0\\ ");
            await TestValid("#,##0_);[Red]\\(#,##0\\)");
            await TestValid("#,##0_р_.;[Red]\\-#,##0_р_.");
            await TestValid("#,##0_р_.;\\-#,##0_р_.");
            await TestValid("#.0000,,");
            await TestValid("#0");
            await TestValid("#0.00");
            await TestValid("#0.0000");
            await TestValid("#\\ ?/10");
            await TestValid("#\\ ?/2");
            await TestValid("#\\ ?/4");
            await TestValid("#\\ ?/8");
            await TestValid("#\\ ?/?");
            await TestValid("#\\ ??/100");
            await TestValid("#\\ ??/100;[Red]\\(#\\ ??/16\\)");
            await TestValid("#\\ ??/16");
            await TestValid("#\\ ??/??");
            await TestValid("#\\ ??/?????????");
            await TestValid("#\\ ???/???");
            await TestValid("**\\ #,###,#00,000.00,**");
            await TestValid("0");
            await TestValid("0\"abde\".0\"??\"000E+00");
            await TestValid("0%");
            await TestValid("0.0");
            await TestValid("0.0%");
            await TestValid("0.00");
            await TestValid("0.00\"°\"");
            await TestValid("0.00%");
            await TestValid("0.000");
            await TestValid("0.000%");
            await TestValid("0.0000");
            await TestValid("0.000000");
            await TestValid("0.00000000");
            await TestValid("0.000000000");
            await TestValid("0.000000000%");
            await TestValid("0.00000000000");
            await TestValid("0.000000000000000");
            await TestValid("0.00000000E+00");
            await TestValid("0.0000E+00");
            await TestValid("0.00;[Red]0.00");
            await TestValid("0.00E+00");
            await TestValid("0.00_);[Red]\\(0.00\\)");
            await TestValid("0.00_);\\(0.00\\)");
            await TestValid("0.0_ ");
            await TestValid("00.00.00.000");
            await TestValid("00.000%");
            await TestValid("0000");
            await TestValid("00000");
            await TestValid("00000000");
            await TestValid("000000000");
            await TestValid("00000\\-0000");
            await TestValid("00000\\-00000");
            await TestValid("000\\-00\\-0000");
            await TestValid("0;[Red]0");
            await TestValid("0\\-00000\\-00000\\-0");
            await TestValid("0_);[Red]\\(0\\)");
            await TestValid("0_);\\(0\\)");
            await TestValid("@");
            await TestValid("A/P");
            await TestValid("AM/PM");
            await TestValid("AM/PMh\"時\"mm\"分\"ss\"秒\";@");
            await TestValid("D");
            await TestValid("DD");
            await TestValid("DD/MM/YY;@");
            await TestValid("DD/MM/YYYY");
            await TestValid("DD/MM/YYYY;@");
            await TestValid("DDD");
            await TestValid("DDDD");
            await TestValid("DDDD\", \"MMMM\\ DD\", \"YYYY");
            await TestValid("GENERAL");
            await TestValid("General");
            await TestValid("H");
            await TestValid("H:MM:SS\\ AM/PM");
            await TestValid("HH:MM");
            await TestValid("HH:MM:SS\\ AM/PM");
            await TestValid("HHM");
            await TestValid("HHMM");
            await TestValid("HH[MM]");
            await TestValid("HH[M]");
            await TestValid("M/D/YYYY");
            await TestValid("M/D/YYYY\\ H:MM");
            await TestValid("MM/DD/YY");
            await TestValid("S");
            await TestValid("SS");
            await TestValid("YY");
            await TestValid("YYM");
            await TestValid("YYMM");
            await TestValid("YYMMM");
            await TestValid("YYMMMM");
            await TestValid("YYMMMMM");
            await TestValid("YYYY");
            await TestValid("YYYY-MM-DD HH:MM:SS");
            await TestValid("YYYY\\-MM\\-DD");
            await TestValid("[$$-409]#,##0");
            await TestValid("[$$-409]#,##0.00");
            await TestValid("[$$-409]#,##0.00_);[Red]\\([$$-409]#,##0.00\\)");
            await TestValid("[$$-C09]#,##0.00");
            await TestValid("[$-100042A]h:mm:ss\\ AM/PM;@");
            await TestValid("[$-1010409]0.000%");
            await TestValid("[$-1010409]General");
            await TestValid("[$-1010409]d/m/yyyy\\ h:mm\\ AM/PM;@");
            await TestValid("[$-1010409]dddd, mmmm dd, yyyy");
            await TestValid("[$-1010409]m/d/yyyy");
            await TestValid("[$-1409]h:mm:ss\\ AM/PM;@");
            await TestValid("[$-2000000]h:mm:ss;@");
            await TestValid("[$-2010401]d/mm/yyyy\\ h:mm\\ AM/PM;@");
            await TestValid("[$-4000439]h:mm:ss\\ AM/PM;@");
            await TestValid("[$-4010439]d/m/yyyy\\ h:mm\\ AM/PM;@");
            await TestValid("[$-409]AM/PM\\ hh:mm:ss;@");
            await TestValid("[$-409]d/m/yyyy\\ hh:mm;@");
            await TestValid("[$-409]d\\-mmm;@");
            await TestValid("[$-409]d\\-mmm\\-yy;@");
            await TestValid("[$-409]d\\-mmm\\-yyyy;@");
            await TestValid("[$-409]dd/mm/yyyy\\ hh:mm;@");
            await TestValid("[$-409]dd\\-mmm\\-yy;@");
            await TestValid("[$-409]h:mm:ss\\ AM/PM;@");
            await TestValid("[$-409]h:mm\\ AM/PM;@");
            await TestValid("[$-409]m/d/yy\\ h:mm\\ AM/PM;@");
            await TestValid("[$-409]mmm\\-yy;@");
            await TestValid("[$-409]mmmm\\ d\\,\\ yyyy;@");
            await TestValid("[$-409]mmmm\\-yy;@");
            await TestValid("[$-409]mmmmm;@");
            await TestValid("[$-409]mmmmm\\-yy;@");
            await TestValid("[$-40E]h\\ \"óra\"\\ m\\ \"perckor\"\\ AM/PM;@");
            await TestValid("[$-412]AM/PM\\ h\"시\"\\ mm\"분\"\\ ss\"초\";@");
            await TestValid("[$-41C]h:mm:ss\\.AM/PM;@");
            await TestValid("[$-449]hh:mm:ss\\ AM/PM;@");
            await TestValid("[$-44E]hh:mm:ss\\ AM/PM;@");
            await TestValid("[$-44F]hh:mm:ss\\ AM/PM;@");
            await TestValid("[$-D000409]h:mm\\ AM/PM;@");
            await TestValid("[$-D010000]d/mm/yyyy\\ h:mm\\ \"น.\";@");
            await TestValid("[$-F400]h:mm:ss\\ AM/PM");
            await TestValid("[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy");
            await TestValid("[$AUD]\\ #,##0.00");
            await TestValid("[$RD$-1C0A]#,##0.00;[Red]\\-[$RD$-1C0A]#,##0.00");
            await TestValid("[$SFr.-810]\\ #,##0.00_);[Red]\\([$SFr.-810]\\ #,##0.00\\)");
            await TestValid("[$£-809]#,##0.00;[Red][$£-809]#,##0.00");
            await TestValid("[$¥-411]#,##0.00");
            await TestValid("[$¥-804]#,##0.00");
            await TestValid("[<0]\"\";0%");
            await TestValid("[<=9999999]###\\-####;\\(###\\)\\ ###\\-####");
            await TestValid("[=0]?;#,##0.00");
            await TestValid("[=0]?;0%");
            await TestValid("[=0]?;[<4.16666666666667][hh]:mm:ss;[hh]:mm");
            await TestValid("[>999999]#,,\"M\";[>999]#,\"K\";#");
            await TestValid("[>999999]#.000,,\"M\";[>999]#.000,\"K\";#.000");
            await TestValid("[>=100000]0.000\\ \\\";[Red]0.000\\ \\<\\ \\>\\ \\\"\\ \\&\\ \\'\\ ");
            await TestValid("[>=100000]0.000\\ \\<;[Red]0.000\\ \\>");
            await TestValid("[BLACK]@");
            await TestValid("[BLUE]GENERAL");
            await TestValid("[Black]@");
            await TestValid("[Blue]General");
            await TestValid("[CYAN]@");
            await TestValid("[Cyan]@");
            await TestValid("[DBNum1][$-804]AM/PMh\"时\"mm\"分\";@");
            await TestValid("[DBNum1][$-804]General");
            await TestValid("[DBNum1][$-804]h\"时\"mm\"分\";@");
            await TestValid("[ENG][$-1004]dddd\\,\\ d\\ mmmm\\,\\ yyyy;@");
            await TestValid("[ENG][$-101040D]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-101042A]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-140C]dddd\\ \"YeahWoo!\"\\ ddd\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-2C0A]dddd\\ d\" de \"mmmm\" de \"yyyy;@");
            await TestValid("[ENG][$-402]dd\\ mmmm\\ yyyy\\ \"г.\";@");
            await TestValid("[ENG][$-403]dddd\\,\\ d\" / \"mmmm\" / \"yyyy;@");
            await TestValid("[ENG][$-405]d\\.\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-408]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-409]d\\-mmm;@");
            await TestValid("[ENG][$-409]d\\-mmm\\-yy;@");
            await TestValid("[ENG][$-409]d\\-mmm\\-yyyy;@");
            await TestValid("[ENG][$-409]dd\\-mmm\\-yy;@");
            await TestValid("[ENG][$-409]mmm\\-yy;@");
            await TestValid("[ENG][$-409]mmmm\\ d\\,\\ yyyy;@");
            await TestValid("[ENG][$-409]mmmm\\-yy;@");
            await TestValid("[ENG][$-40B]d\\.\\ mmmm\\t\\a\\ yyyy;@");
            await TestValid("[ENG][$-40C]d/mmm/yyyy;@");
            await TestValid("[ENG][$-40E]yyyy/\\ mmmm\\ d\\.;@");
            await TestValid("[ENG][$-40F]dd\\.\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-410]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-415]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-416]d\\ \\ mmmm\\,\\ yyyy;@");
            await TestValid("[ENG][$-418]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-41A]d\\.\\ mmmm\\ yyyy\\.;@");
            await TestValid("[ENG][$-41B]d\\.\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-41D]\"den \"\\ d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-420]dddd\\,\\ dd\\ mmmm\\,\\ yyyy;@");
            await TestValid("[ENG][$-421]dd\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-424]dddd\\,\\ d\\.\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-425]dddd\\,\\ d\\.\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-426]dddd\\,\\ yyyy\". gada \"d\\.\\ mmmm;@");
            await TestValid("[ENG][$-427]yyyy\\ \"m.\"\\ mmmm\\ d\\ \"d.\";@");
            await TestValid("[ENG][$-42B]dddd\\,\\ d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-42C]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-42D]yyyy\"(e)ko\"\\ mmmm\"ren\"\\ d\"a\";@");
            await TestValid("[ENG][$-42F]dddd\\,\\ dd\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-437]yyyy\\ \\წ\\ლ\\ი\\ს\\ dd\\ mm\\,\\ dddd;@");
            await TestValid("[ENG][$-438]d\\.\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-43F]d\\ mmmm\\ yyyy\\ \"ж.\";@");
            await TestValid("[ENG][$-444]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-449]dd\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-44E]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-44F]dd\\ mmmm\\ yyyy\\ dddd;@");
            await TestValid("[ENG][$-457]dd\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-813]dddd\\ d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-81A]dddd\\,\\ d\\.\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-82C]d\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-843]yyyy\\ \"й\"\"и\"\"л\"\\ d/mmmm;@");
            await TestValid("[ENG][$-C07]dddd\\,\\ dd\\.\\ mmmm\\ yyyy;@");
            await TestValid("[ENG][$-FC19]yyyy\\,\\ dd\\ mmmm;@");
            await TestValid("[ENG][$-FC22]d\\ mmmm\\ yyyy\" р.\";@");
            await TestValid("[ENG][$-FC23]d\\ mmmm\\ yyyy;@");
            await TestValid("[GREEN]#,###");
            await TestValid("[Green]#,###");
            await TestValid("[HH]");
            await TestValid("[HIJ][$-2060401]d/mm/yyyy\\ h:mm\\ AM/PM;@");
            await TestValid("[HIJ][$-2060401]d\\ mmmm\\ yyyy;@");
            await TestValid("[H]");
            await TestValid("[JPN][$-411]gggyy\"年\"m\"月\"d\"日\"\\ dddd;@");
            await TestValid("[MAGENTA]0.00");
            await TestValid("[Magenta]0.00");
            await TestValid("[RED]#.##");
            await TestValid("[Red]#.##");
            await TestValid("[Red][<-25]General;[Blue][>25]General;[Green]General;[Yellow]General\\ ");
            await TestValid("[Red][<=-25]General;[Blue][>=25]General;[Green]General;[Yellow]General");
            await TestValid("[Red][<>50]General;[Blue]000");
            await TestValid("[Red][=50]General;[Blue]000");
            await TestValid("[SS]");
            await TestValid("[S]");
            await TestValid("[TWN][DBNum1][$-404]y\"年\"m\"月\"d\"日\";@");
            await TestValid("[WHITE]0.0");
            await TestValid("[White]0.0");
            await TestValid("[YELLOW]@");
            await TestValid("[Yellow]@");
            await TestValid("[h]");
            await TestValid("[h]:mm:ss");
            await TestValid("[h]:mm:ss;@");
            await TestValid("[h]\\.mm\" Uhr \";@");
            await TestValid("[hh]");
            await TestValid("[s]");
            await TestValid("[ss]");
            await TestValid("\\#\\r\\e\\c");
            await TestValid("\\$#,##0_);[Red]\"($\"#,##0\\)");
            await TestValid("\\$0.00");
            await TestValid("\\C\\O\\B\\ \\o\\n\\ @");
            await TestValid("\\C\\R\\O\\N\\T\\A\\B\\ \\o\\n\\ @");
            await TestValid("\\R\\e\\s\\u\\l\\t\\ \\o\\n\\ @");
            await TestValid("\\S\\Q\\L\\ \\:\\ @");
            await TestValid("\\S\\Q\\L\\ \\R\\e\\q\\u\\e\\s\\t\\ \\f\\o\\r\\ @");
            await TestValid("\\c\\c\\c?????0\"aaaa\"0\"bbbb\"000000.00%");
            await TestValid("\\u\\n\\t\\i\\l\\ h:mm;@");
            await TestValid("_ \"￥\"* #,##0.00_ \"Positive\";_ \"￥\"* \\-#,##0.00_ ;_ \"￥\"* \"-\"??_ \"Negtive\";_ @_ \\ \"Zero\"");
            await TestValid("_ * #,##0.00_)[$﷼-429]_ ;_ * \\(#,##0.00\\)[$﷼-429]_ ;_ * \"-\"??_)[$﷼-429]_ ;_ @_ ");
            await TestValid("_ * #,##0_ ;_ * \\-#,##0_ ;[Red]_ * \"-\"_ ;_ @_ ");
            await TestValid("_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)");
            await TestValid("_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"??_);_(@_)");
            await TestValid("_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"_);_(@_)");
            await TestValid("_(* #,##0.0000_);_(* \\(#,##0.0000\\);_(* \"-\"??_);_(@_)");
            await TestValid("_(* #,##0.000_);_(* \\(#,##0.000\\);_(* \"-\"??_);_(@_)");
            await TestValid("_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)");
            await TestValid("_(* #,##0.0_);_(* \\(#,##0.0\\);_(* \"-\"??_);_(@_)");
            await TestValid("_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"??_);_(@_)");
            await TestValid("_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)");
            await TestValid("_([$ANG]\\ * #,##0.0_);_([$ANG]\\ * \\(#,##0.0\\);_([$ANG]\\ * \"-\"?_);_(@_)");
            await TestValid("_-\"€\"\\ * #,##0.00_-;_-\"€\"\\ * #,##0.00\\-;_-\"€\"\\ * \"-\"??_-;_-@_-");
            await TestValid("_-* #,##0.00\" TL\"_-;\\-* #,##0.00\" TL\"_-;_-* \\-??\" TL\"_-;_-@_-");
            await TestValid("_-* #,##0.00\" €\"_-;\\-* #,##0.00\" €\"_-;_-* \\-??\" €\"_-;_-@_-");
            await TestValid("_-* #,##0.00\\ \"р.\"_-;\\-* #,##0.00\\ \"р.\"_-;_-* \"-\"??\\ \"р.\"_-;_-@_-");
            await TestValid("_-* #,##0.00\\ \"€\"_-;\\-* #,##0.00\\ \"€\"_-;_-* \"-\"??\\ \"€\"_-;_-@_-");
            await TestValid("_-* #,##0.00\\ [$€-407]_-;\\-* #,##0.00\\ [$€-407]_-;_-* \\-??\\ [$€-407]_-;_-@_-");
            await TestValid("_-* #,##0.0\\ _F_-;\\-* #,##0.0\\ _F_-;_-* \"-\"??\\ _F_-;_-@_-");
            await TestValid("_-* #,##0\\ \"€\"_-;\\-* #,##0\\ \"€\"_-;_-* \"-\"\\ \"€\"_-;_-@_-");
            await TestValid("_-* #,##0_-;\\-* #,##0_-;_-* \"-\"??_-;_-@_-");
            await TestValid("_-\\$* #,##0.0_ ;_-\\$* \\-#,##0.0\\ ;_-\\$* \"-\"?_ ;_-@_ ");
            await TestValid("d");
            await TestValid("d-mmm");
            await TestValid("d-mmm-yy");
            await TestValid("d/m");
            await TestValid("d/m/yy;@");
            await TestValid("d/m/yyyy;@");
            await TestValid("d/mm/yy;@");
            await TestValid("d/mm/yyyy;@");
            await TestValid("d\\-mmm");
            await TestValid("d\\-mmm\\-yyyy");
            await TestValid("dd");
            await TestValid("dd\"-\"mmm\"-\"yyyy");
            await TestValid("dd/m/yyyy");
            await TestValid("dd/mm/yy");
            await TestValid("dd/mm/yy;@");
            await TestValid("dd/mm/yy\\ hh:mm");
            await TestValid("dd/mm/yyyy");
            await TestValid("dd/mm/yyyy\\ hh:mm:ss");
            await TestValid("dd/mmm");
            await TestValid("dd\\-mm\\-yy");
            await TestValid("dd\\-mmm\\-yy");
            await TestValid("dd\\-mmm\\-yyyy\\ hh:mm:ss.000");
            await TestValid("dd\\/mm\\/yy");
            await TestValid("dd\\/mm\\/yyyy");
            await TestValid("ddd");
            await TestValid("dddd");
            await TestValid("dddd, mmmm dd, yyyy");
            await TestValid("h");
            await TestValid("h\"时\"mm\"分\"ss\"秒\";@");
            await TestValid("h\"時\"mm\"分\"ss\"秒\";@");
            await TestValid("h:mm");
            await TestValid("h:mm AM/PM");
            await TestValid("h:mm:ss");
            await TestValid("h:mm:ss AM/PM");
            await TestValid("h:mm:ss;@");
            await TestValid("h:mm;@");
            await TestValid("h\\.mm\" Uhr \";@");
            await TestValid("h\\.mm\" h\";@");
            await TestValid("h\\.mm\" u.\";@");
            await TestValid("hh\":\"mm AM/PM");
            await TestValid("hh:mm:ss");
            await TestValid("hh:mm:ss\\ AM/PM");
            await TestValid("hh\\.mm\" h\";@");
            await TestValid("hhm");
            await TestValid("hhmm");
            await TestValid("m\"月\"d\"日\"");
            await TestValid("m/d/yy");
            await TestValid("m/d/yy h:mm");
            await TestValid("m/d/yy;@");
            await TestValid("m/d/yy\\ h:mm");
            await TestValid("m/d/yy\\ h:mm;@");
            await TestValid("m/d/yyyy");
            await TestValid("m/d/yyyy;@");
            await TestValid("m/d/yyyy\\ h:mm:ss;@");
            await TestValid("m/d;@");
            await TestValid("m\\/d\\/yyyy");
            await TestValid("mm/dd");
            await TestValid("mm/dd/yy");
            await TestValid("mm/dd/yy;@");
            await TestValid("mm/dd/yyyy");
            await TestValid("mm:ss");
            await TestValid("mm:ss.0;@");
            await TestValid("mmm d, yyyy");
            await TestValid("mmm\" \"d\", \"yyyy");
            await TestValid("mmm-yy");
            await TestValid("mmm-yy;@");
            await TestValid("mmm/yy");
            await TestValid("mmm\\-yy");
            await TestValid("mmm\\-yy;@");
            await TestValid("mmm\\-yyyy");
            await TestValid("mmmm\\ d\\,\\ yyyy");
            await TestValid("mmmm\\ yyyy");
            await TestValid("mmss.0");
            await TestValid("s");
            await TestValid("ss");
            await TestValid("yy");
            await TestValid("yy/mm/dd");
            await TestValid("yy\\.mm\\.dd");
            await TestValid("yym");
            await TestValid("yymm");
            await TestValid("yymmm");
            await TestValid("yymmmm");
            await TestValid("yymmmmm");
            await TestValid("yyyy");
            await TestValid("yyyy\"년\"\\ m\"월\"\\ d\"일\";@");
            await TestValid("yyyy-m-d h:mm AM/PM");
            await TestValid("yyyy-mm-dd");
            await TestValid("yyyy/mm/dd");
            await TestValid("yyyy\\-m\\-d\\ hh:mm:ss");
            await TestValid("yyyy\\-mm\\-dd");
            await TestValid("yyyy\\-mm\\-dd;@");
            await TestValid("yyyy\\-mm\\-dd\\ h:mm");
            await TestValid("yyyy\\-mm\\-dd\\Thh:mm");
            await TestValid("yyyy\\-mm\\-dd\\Thhmmss.000");
        }

        [Test]
        [Arguments((string)null)]
        [Arguments("")]
        [Arguments("General")]
        public async Task TestDefaultFormatString(string formatString)
        {
            string result;

            result = Format(1234.56, formatString, CultureInfo.InvariantCulture);
            await Assert.That(result).IsEqualTo("1234.56");

            result = Format(Double.MaxValue, formatString, CultureInfo.InvariantCulture);
            await Assert.That(result).IsEqualTo("1.79769313486232E+308");

            result = Format(float.MaxValue, formatString, CultureInfo.InvariantCulture);
            await Assert.That(result).IsEqualTo("3.402823E+38");

            result = Format(new DateTime(2017, 10, 28), formatString, new CultureInfo("sv-se"));
            await Assert.That(result).IsEqualTo("2017-10-28 00:00:00");

            result = Format(new DateTime(2017, 10, 28), formatString, CultureInfo.InvariantCulture);
            await Assert.That(result).IsEqualTo("10/28/2017 00:00:00");
        }

        [Test]
        public async Task TestCurrency()
        {
            await Test(1234.56, "[$€-1809]# ##0.00", "€1 234.56");
            await Test(1234.56, "#,##0.00 [$EUR]", "1,234.56 EUR");
        }
    }
}
