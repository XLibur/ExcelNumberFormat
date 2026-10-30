using System;
using System.Globalization;
using System.Threading.Tasks;
using static Xlibur.ExcelNumberFormat.Tests.FormatHelper;

// Much of the test data was adapted from the SheetJS/ssf project.
// Copyright (C) 2013-present  SheetJS. 
// Licensed under the Apache License, Version 2.0

namespace Xlibur.ExcelNumberFormat.Tests
{
    public class ConditionTests
    {
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
    }
}
