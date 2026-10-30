using System;
using System.Globalization;
using System.Threading.Tasks;
using static ExcelNumberFormat.Tests.FormatHelper;

// Much of the test data was adapted from the SheetJS/ssf project.
// Copyright (C) 2013-present  SheetJS. 
// Licensed under the Apache License, Version 2.0

namespace ExcelNumberFormat.Tests
{
    public class TextTests
    {
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
    }
}
