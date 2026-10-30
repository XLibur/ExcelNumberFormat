# Xlibur.ExcelNumberFormat

.NET library to parse ECMA-376 number format strings and format values like Excel and other spreadsheet softwares.

> This library is a fork of [andersnm/ExcelNumberFormat](https://github.com/andersnm/ExcelNumberFormat). It is published to NuGet as `Xlibur.ExcelNumberFormat`; the namespace is unchanged.
>
> Credit to the original author, [@andersnm](https://github.com/andersnm), who wrote the library this is built on. The original copyright and MIT license are kept in the [LICENSE](https://github.com/XLibur/ExcelNumberFormat/blob/main/LICENSE) file.

## Install

```
dotnet add package Xlibur.ExcelNumberFormat
```

## Usage

```C#
using System.Globalization;
using ExcelNumberFormat;

var format = new NumberFormat("#,##0.00");
Console.WriteLine(format.Format(1234.5, CultureInfo.InvariantCulture)); // 1,234.50

// Dates and durations
Console.WriteLine(new NumberFormat("yyyy-mm-dd hh:mm").Format(45498.5, CultureInfo.InvariantCulture)); // 2024-07-25 12:00
Console.WriteLine(new NumberFormat("[h]:mm:ss").Format(TimeSpan.FromMinutes(90), CultureInfo.InvariantCulture)); // 1:30:00
```

Parsing a format string costs more than formatting a value with it. If the same format strings come up again and again, for example once per cell, get them from `NumberFormat.GetOrCreate(formatString)`, which parses each format string only the first time. A `NumberFormat` is immutable and safe to share between threads.

## Features

- Parses and formats most custom number formats as expected: decimal, percent, thousands, exponential, fraction, currency, date/time, duration, text.
- Supports multiple sections with conditions, including empty sections such as `;;;`.
- Formats values with relevant constants from `CultureInfo`.
- Supports `DateTime`, `TimeSpan` and numeric values for date and duration formats.
- Supports both 1900- and 1904-based numeric datetimes (Excel on Mac uses 1904-based dates).
- Rounds like Excel: half away from zero, and fractional seconds (`.0`, `.00`, `.000`) are rounded and carried.
- Targets net8.0, net10.0 and net11.0.

## Formatting .NET types

`Format()` takes a value of type `object`, and has overloads for `double`, `float`, `decimal`, `long`, `ulong`, `DateTime` and `TimeSpan` that avoid boxing the value. Other numeric types (`int`, `short`, `byte` and so on) bind to the `long` overload. `DateOnly` and `TimeOnly` are accepted through the `object` overload. Internally, the value is converted to a specific .NET type depending on the kind of number format:

Format Kind | Example | .NET type | Conversion
-|-|-|-
Number   | 0.00      | double   | numeric value
Fraction | 0/0       | double   | numeric value
Exponent | \#0.0E+0  | double   | numeric value
Date/Time| hh\:mm    | DateTime | `DateTime` as is, numeric values as an Excel date serial
Duration | \[hh\]\:mm| TimeSpan | `TimeSpan` as is, numeric values as a number of days
General  | General   | (any)    | the value as text
Text     | ;;;"Text: "@ | string | the string

If a value cannot be formatted, for example a date format applied to a string, `Format()` returns the value as plain text instead of throwing.

## Links

- [Source and issues](https://github.com/XLibur/ExcelNumberFormat)
- [Changelog](https://github.com/XLibur/ExcelNumberFormat/blob/main/CHANGELOG.md)
