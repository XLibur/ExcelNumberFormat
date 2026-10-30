using System;
using System.Globalization;
using System.Threading.Tasks;
using static ExcelNumberFormat.Tests.FormatHelper;

// The expected values were produced by Excel 16 with the same format strings.

namespace ExcelNumberFormat.Tests
{
    public class EraTests
    {
        private const string Full = "[$-411]ggge\"年\"m\"月\"d\"日\";@";

        [Test]
        public async Task TestJapaneseEraWithKanji()
        {
            // https://github.com/andersnm/ExcelNumberFormat/issues/39
            await Test(44259, Full, "令和3年3月4日");
            await Test(new DateTime(1900, 3, 1), Full, "明治33年3月1日");
            await Test(new DateTime(1912, 7, 29), Full, "明治45年7月29日");
            await Test(new DateTime(1912, 7, 30), Full, "大正1年7月30日");
            await Test(new DateTime(1926, 12, 24), Full, "大正15年12月24日");
            await Test(new DateTime(1926, 12, 25), Full, "昭和1年12月25日");
            await Test(new DateTime(1989, 1, 7), Full, "昭和64年1月7日");
            await Test(new DateTime(1989, 1, 8), Full, "平成1年1月8日");
            await Test(new DateTime(2019, 4, 30), Full, "平成31年4月30日");
            await Test(new DateTime(2019, 5, 1), Full, "令和1年5月1日");
        }

        [Test]
        public async Task TestJapaneseEraNames()
        {
            var date = new DateTime(2021, 3, 4);
            await Test(date, "[$-411]g", "R");
            await Test(date, "[$-411]gg", "令");
            await Test(date, "[$-411]ggg", "令和");
            await Test(date, "[$-411]gggg", "令和");
            await Test(new DateTime(1912, 7, 30), "[$-411]g", "T");
            await Test(new DateTime(1926, 12, 25), "[$-411]gg", "昭");
            await Test(new DateTime(1989, 1, 8), "[$-411]g", "H");
            await Test(new DateTime(1900, 3, 1), "[$-411]g", "M");
        }

        [Test]
        public async Task TestJapaneseEraYear()
        {
            await Test(44259, @"[$-411]ge\.m\.d;@", "R3.3.4");
            await Test(new DateTime(2019, 5, 1), @"[$-411]ge\.m\.d;@", "R1.5.1");
            await Test(new DateTime(2021, 3, 4), "[$-411]e", "3");
            await Test(new DateTime(2021, 3, 4), "[$-411]ee", "03");
            await Test(new DateTime(2021, 3, 4), "[$-411]eee", "03");
            await Test(new DateTime(1912, 7, 30), "[$-411]ee", "01");
            await Test(new DateTime(1989, 1, 7), "[$-411]e", "64");
            await Test(new DateTime(2021, 3, 4), "[$-411]yyyy e", "2021 3");
            await Test(new DateTime(2021, 3, 4), "[$-411]yy e", "21 3");
            await Test(new DateTime(2021, 3, 4), "[$-411]h:mm ge", "0:00 R3");
            await Test(new DateTime(2021, 3, 4), "[$-411]gggee", "令和03");
        }

        [Test]
        public async Task TestEraYearBeforeSeparator()
        {
            // "e-" and "e+" are exponents in a number, but an era year and a literal in a date
            await Test(new DateTime(2021, 3, 4), "[$-411]e-m-d", "3-3-4");
            await Test(new DateTime(2021, 3, 4), "[$-411]e+m", "3+3");
            await Test(new DateTime(2021, 3, 4), "[$-411]e/m/d", "3/3/4");
            await Test(new DateTime(2021, 3, 4), "e-m-d", "2021-3-4");
        }

        [Test]
        public async Task TestEraYearOutsideJapanese()
        {
            // Without the Japanese locale e and ee print the four digit year
            await Test(new DateTime(2021, 3, 4), "e", "2021");
            await Test(new DateTime(2021, 3, 4), "ee", "2021");
            await Test(new DateTime(2021, 3, 4), "[$-409]e", "2021");
            await Test(new DateTime(2021, 3, 4), "[$-409]ee", "2021");
            await Test(new DateTime(2021, 3, 4), "e/m/d", "2021/3/4");
        }

        [Test]
        public async Task TestEraYearIsDate()
        {
            await Assert.That(IsDateFormatString("[$-411]ge.m.d")).IsTrue();
            await Assert.That(IsDateFormatString("e")).IsTrue();
        }

        [Test]
        public async Task TestUpperCaseEraYearIsInvalid()
        {
            // Excel rejects an upper case E as a date code
            await Assert.That(new NumberFormat("[$-411]E").IsValid).IsFalse();
        }

        [Test]
        public async Task TestScientificFormatsAreUnchanged()
        {
            await Test(12345.678, "0.00E+00", "1.23E+04");
            await Test(0.000123, "0.00E-00", "1.23E-04");
        }
    }
}
