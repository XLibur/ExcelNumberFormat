using System;
using System.Globalization;
using System.Threading.Tasks;
using static Xlibur.ExcelNumberFormat.Tests.FormatHelper;

// Much of the test data was adapted from the SheetJS/ssf project.
// Copyright (C) 2013-present  SheetJS. 
// Licensed under the Apache License, Version 2.0

namespace Xlibur.ExcelNumberFormat.Tests
{
    public class FractionTests
    {
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

        [Test]
        public async Task TestFractionConstantDenominator()
        {
            // Expected values were checked against Excel 16
            await Test(-0.25, "?/4", "-1/4");
            await Test(1.75, "?/4", "7/4");
            await Test(1.75, "# ?/4", "1 3/4");
            await Test(-1.75, "# ?/4", "-1 3/4");
        }

        [Test]
        public async Task TestFractionRoundingCarry()
        {
            await Test(2.99, "# ?/4", "3    ");
            await Test(2.9999999, "# ?/?", "3    ");
            await Test(0.9999999, "?/?", "1/1");
        }

        [Test]
        public async Task TestFractionSignInSections()
        {
            await Test(-0.5, "?/?", "-1/2");
            await Test(-0.5, "?/?;?/?", "1/2");
            await Test(-2.5, "# ?/?", "-2 1/2");
            await Test(-2.5, "# ?/?;# ?/?", "2 1/2");
        }

        [Test]
        public async Task TestFractionLargeValues()
        {
            // Excel shows ######## here; the whole part must not wrap around like a 32-bit int
            await Test(3e9, "# ?/?", "3000000000    ");
        }
    }
}
