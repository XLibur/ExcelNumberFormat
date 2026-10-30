using System;
using System.Globalization;
using System.Threading.Tasks;
using static ExcelNumberFormat.Tests.FormatHelper;

// Much of the test data was adapted from the SheetJS/ssf project.
// Copyright (C) 2013-present  SheetJS. 
// Licensed under the Apache License, Version 2.0

namespace ExcelNumberFormat.Tests
{
    public class FormatStringTests
    {
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
        public async Task TestGetOrCreateReusesFormats()
        {
            var first = NumberFormat.GetOrCreate("#,##0.00");
            var second = NumberFormat.GetOrCreate("#,##0.00");
            var other = NumberFormat.GetOrCreate("0.0%");

            await Assert.That(second).IsSameReferenceAs(first);
            await Assert.That(other).IsNotSameReferenceAs(first);
            await Assert.That(first.FormatString).IsEqualTo("#,##0.00");
            await Assert.That(first.Format(1234.5, CultureInfo.InvariantCulture)).IsEqualTo("1,234.50");
            await Assert.That(other.Format(0.256, CultureInfo.InvariantCulture)).IsEqualTo("25.6%");
        }

        [Test]
        public async Task TestGetOrCreateIsBounded()
        {
            // More distinct formats than the cache holds: every one still formats correctly
            for (var i = 0; i < 600; i++)
            {
                var format = NumberFormat.GetOrCreate("0\"#" + i + "\"");
                await Assert.That(format.Format(7, CultureInfo.InvariantCulture)).IsEqualTo("7#" + i);
            }
        }

        [Test]
        public async Task TestGetOrCreateInvalidAndNull()
        {
            await Assert.That(NumberFormat.GetOrCreate("[<]0").IsValid).IsFalse();
            await Assert.That(NumberFormat.GetOrCreate(null)).IsNotNull();
        }

        [Test]
        public async Task TestEmptySections()
        {
            // Expected values were checked against Excel 16
            await Test(-5, "0;;0", "");
            await Test(0, "0;;0", "0");
            await Test(-5, "0;", "");
            await Test(0, "0;;", "");
            await Test(5, ";;;", "");
            await Test(-5, ";;;", "");
            await Test(0, ";;;", "");
            await Test("abc", ";;;", "");
            await Test(-5, "0.00;;\"zero\"", "");
            await Test(0, "0.00;;\"zero\"", "zero");
            await Test(5, "0.00;;\"zero\"", "5.00");
        }

        [Test]
        [Arguments("[<]0")]
        [Arguments("[>-]0")]
        [Arguments("[=.]0")]
        public async Task TestConditionWithoutNumberIsInvalid(string formatString)
        {
            var format = new NumberFormat(formatString);
            await Assert.That(format.IsValid).IsFalse();
        }

        [Test]
        public async Task TestFractionConstantDenominatorTooLarge()
        {
            // Does not throw from the constructor
            var format = new NumberFormat("# ?/99999999999");
            await Assert.That(format.IsValid).IsFalse();
        }
    }
}
