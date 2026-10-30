using System;
using System.Globalization;
using System.Threading.Tasks;
using static Xlibur.ExcelNumberFormat.Tests.FormatHelper;

// Much of the test data was adapted from the SheetJS/ssf project.
// Copyright (C) 2013-present  SheetJS. 
// Licensed under the Apache License, Version 2.0

namespace Xlibur.ExcelNumberFormat.Tests
{
    public class NumberTests
    {
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
        public async Task TestOtherNumericTypes()
        {
            await Test((byte)200, "#,##0", "200");
            await Test((sbyte)-5, "0.0", "-5.0");
            await Test((ushort)1234, "#,##0", "1,234");
            await Test(1234u, "#,##0", "1,234");
            await Test(1234L, "#,##0", "1,234");
            await Test(12345678901234UL, "#,##0", "12,345,678,901,234");
            await Test(1234.5f, "#,##0.00", "1,234.50");
            await Test(1234.56f, "0.00", "1234.56");
            await Test(1234m, "#,##0", "1,234");
            await Test(1234.5m, "#,##0.00", "1,234.50");
            await Test(0.25m, "0%", "25%");
            await Test(-1234L, "#,##0;(#,##0)", "(1,234)");
            await Test(45498L, "yyyy-mm-dd", "2024-07-25");
            await Test(0L, "#,##0;-#,##0;\"zero\"", "zero");
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
        public async Task TestCurrency()
        {
            await Test(1234.56, "[$€-1809]# ##0.00", "€1 234.56");
            await Test(1234.56, "#,##0.00 [$EUR]", "1,234.56 EUR");
        }

        [Test]
        public async Task TestExponentZeroAndCarry()
        {
            // Expected values were checked against Excel 16
            await Test(0, "0.00E+00", "0.00E+00");
            await Test(9.999, "0.00E+00", "1.00E+01");
            await Test(99999, "0.0E+0", "1.0E+5");
            await Test(999.99, "##0.0E+0", "1.0E+3");
            await Test(0.00099999, "0.00E+00", "1.00E-03");
            await Test(-9.999, "0.00E+00", "-1.00E+01");
        }

        [Test]
        public async Task TestExponentSignInSections()
        {
            await Test(-5, "0.0E+00", "-5.0E+00");
            await Test(-5, "0.0E+00;0.0E+00", "5.0E+00");
            await Test(-5, "0.0E+00;-0.0E+00", "-5.0E+00");
        }

        [Test]
        public async Task TestNonFiniteNumbers()
        {
            await Test(double.NaN, "0.00E+00", "NaN");
            await Test(double.PositiveInfinity, "0.00E+00", "Infinity");
            await Test(double.NaN, "# ?/?", "NaN");
            await Test(double.PositiveInfinity, "# ?/?", "Infinity");
            await Test(double.NaN, "yyyy-mm-dd", "NaN");
            await Test(double.PositiveInfinity, "[h]:mm", "Infinity");
        }

        [Test]
        public async Task TestFloatConversion()
        {
            await Test(123456792f, "0", "123456792");
            await Test(1234.56f, "0.00", "1234.56");
            await Test(1234.5678f, "0.0000", "1234.5677");
            await Test(float.MaxValue, "General", "3.402823E+38");
        }
    }
}
