ExcelNumberFormat
=================

.NET library to parse ECMA-376 number format strings and format values like Excel and other spreadsheet softwares.

[![Build and test](https://github.com/XLibur/ExcelNumberFormat/actions/workflows/build.yml/badge.svg)](https://github.com/XLibur/ExcelNumberFormat/actions/workflows/build.yml)

> This library is a fork of [andersnm/ExcelNumberFormat](https://github.com/andersnm/ExcelNumberFormat). It is published to NuGet as `Xlibur.ExcelNumberFormat`; the namespace is unchanged.
>
> Credit to the original author, [@andersnm](https://github.com/andersnm), who wrote the library this is built on. The original copyright and MIT license are kept in [LICENSE](LICENSE).

## Install via NuGet

If you want to include ExcelNumberFormat in your project, you can [install it directly from NuGet](https://www.nuget.org/packages/Xlibur.ExcelNumberFormat)

To install ExcelNumberFormat, run the following command in the Package Manager Console

```
PM> Install-Package Xlibur.ExcelNumberFormat
```

## Usage

```C#
var format = new NumberFormat("#.##");
Console.WriteLine(format.Format(1234.56, CultureInfo.InvariantCulture));
```

### Examples

Each row shows a format string, the value passed to `Format()` and the result with `CultureInfo.InvariantCulture`.

```C#
var text = new NumberFormat("#,##0.00").Format(1234.5678, CultureInfo.InvariantCulture); // "1,234.57"
```

Format | Value | Result
-|-|-
`0.00` | 1234.5678 | `1234.57`
`#,##0.00` | 1234.5678 | `1,234.57`
`$#,##0.00` | 1234.5 | `$1,234.50`
`0%` | 0.256 | `26%`
`0.0%` | 0.256 | `25.6%`
`#,##0,"k"` | 1234567 | `1,235k`
`0.00E+00` | 1234.5678 | `1.23E+03`
`# ?/?` | 1.5 | `1 1/2`
`0/8` | 0.125 | `1/8`
`yyyy-mm-dd` | `new DateTime(2024, 7, 25, 14, 30, 0)` | `2024-07-25`
`dddd, mmmm d, yyyy` | `new DateTime(2024, 7, 25, 14, 30, 0)` | `Thursday, July 25, 2024`
`h:mm AM/PM` | `new DateTime(2024, 7, 25, 14, 30, 0)` | `2:30 PM`
`yyyy-mm-dd hh:mm` | 45498.5 | `2024-07-25 12:00`
`[h]:mm:ss` | `new TimeSpan(1, 2, 3, 4)` | `26:03:04`
`0;(0)` | -5 | `(5)`
`0;-0;"zero"` | 0 | `zero`
`[>=100]"big";"small"` | 150 | `big`
`[>=100]"big";"small"` | 50 | `small`
`;;;"Text: "@` | `"abc"` | `Text: abc`
`General` | 1234.5 | `1234.5`

Use `IsValid` to check a format string. An invalid format, such as `[<]0`, makes `Format()` return the value as plain text.

## Features

- Parses and formats most custom number formats as expected: decimal, percent, thousands, exponential, fraction, currency, date/time, duration, text.
- Supports multiple sections with conditions.
- Formats values with relevant constants from CultureInfo.
- Supports DateTime, TimeSpan and numeric values for date and duration formats.
- Supports both 1900- and 1904-based numeric datetimes (Excel on Mac uses 1904-based dates).
- Targets net8.0, net10.0 and net11.0.

## Formatting .NET types

The `Format()` method takes a value of type `object` as parameter. Internally, the value is cast or converted to a specific .NET type depending on the kind of number format:

Format Kind | Example | .NET type|Conversion strategy
-|-|-|-
Number   | 0.00      |double|Convert.ToDouble()
Fraction | 0/0       |double|Convert.ToDouble()
Exponent | \#0.0E+0  |double|Convert.ToDouble()
Date/Time| hh\:mm    |DateTime|ExcelDateTime.TryConvert()
Duration | \[hh\]\:mm|TimeSpan|Cast or TimeSpan.FromDays()
General  | General   |(any)|CompatibleConvert.ToString()
Text     | ;;;"Text: "@|string|Convert.ToString()

In case of errors, `Format()` returns the value from `CompatibleConvert.ToString()`.

`CompatibleConvert.ToString()` formats floats and doubles with explicit precision, or falls back to `Convert.ToString()` for any other types.
`ExcelDateTime.TryConvert()` uses DateTimes as is, or converts numeric values to a DateTime with adjustments for legacy Excel behaviors.
