# Changelog

## Contents

- [Unreleased](#unreleased)
- [v1.2.1](#v121---2026-10-30)
- [v1.2.0](#v120---2026-10-30)

## Unreleased

### 🐛 Bug Fixes

- **`AM/PM` and `A/P` use the locale's designators.** They always printed `AM`, `PM`, `A` and `P`. Now the locale in a `[$-XXX]` code, or the culture if there is none, supplies them, so `[$-405]h:mm AM/PM` prints `dop.` and `odp.`, as Excel does. Without a locale code `A/P` still prints a single letter. With one it prints the full designator, as Excel does ([andersnm/ExcelNumberFormat#45](https://github.com/andersnm/ExcelNumberFormat/issues/45)).

## v1.2.1 - 2026-10-30

### 🐛 Bug Fixes

- Fixed readme

## v1.2.0 - 2026-10-30

### 💥 Breaking Changes

- **The package and assembly are renamed to `Xlibur.ExcelNumberFormat`.** Replace the `ExcelNumberFormat` package reference with `Xlibur.ExcelNumberFormat`. The namespace stays `ExcelNumberFormat`, so no code changes are needed.
- **The library targets `net8.0`, `net10.0` and `net11.0`.** `net20`, `netstandard1.0` and `netstandard2.0` are no longer built.

### ✨ New Features

- **`byte`, `sbyte`, `ushort`, `uint`, `long`, `ulong`, `float`, `decimal`, `DateOnly` and `TimeOnly` values are formatted.** Only `double`, `int` and `short` were treated as numbers, so any other type came back as plain text. `float` goes through its shortest round-trip string, so `1234.56f` formats as `1234.56` ([#5](https://github.com/XLibur/ExcelNumberFormat/issues/5)).
- **`[$-F800]` and `[$-F400]` format with the culture's long date and long time pattern.** Excel ignores the rest of the format for these system codes, and so does the library now ([#3](https://github.com/XLibur/ExcelNumberFormat/issues/3)).
- **`NumberFormat.GetOrCreate(string)` reuses parsed formats.** Parsing a format costs more than formatting a value with it, so callers that see the same format strings again and again, such as once per cell, can share one instance. A `NumberFormat` is immutable and safe to use from several threads. The cache holds at most 256 formats and starts over when full, so it cannot grow without bound.
- **`Format` has overloads for `double`, `float`, `decimal`, `long`, `ulong`, `DateTime` and `TimeSpan`.** They format the value without boxing it and give the same result as the `object` overload. Other numeric types, such as `int` and `short`, bind to the `long` overload. The `ulong` overload is not CLS-compliant.

### 🐛 Bug Fixes

- **Empty sections hide the value.** `;;;`, `0;;0`, `0;` and `0.00;;"zero"` stopped parsing at the first empty section, so later sections were dropped and the value was shown. A trailing `;` now opens an empty section too, and text is hidden by `;;;` as in Excel.
- **Scientific formats handle zero and rounding carries.** `Format(0, "0.00E+00")` threw; it now gives `0.00E+00`. `9.999` gave `10.00E+01` instead of `1.00E+01`, and `99999` with `0.0E+0` gave `10.0E+4` instead of `1.0E+5`.
- **Fractions round and carry like Excel.** `-0.25` with `?/4` gave `-3/4` instead of `-1/4`, `1.75` with `?/4` gave `3/4` instead of `7/4`, and `2.99` with `# ?/4` gave `2` instead of `3`. Whole parts larger than an `int` no longer wrap around.
- **The sign is hidden in the second section of scientific and fraction formats**, as it already was for numbers: `-5` with `0.0E+00;0.0E+00` gives `5.0E+00`.
- **Malformed input no longer throws.** A condition without a number (`[<]0`) makes the format invalid instead of throwing from the constructor, as does a denominator constant too large for an `int`. NaN, infinity and date serials outside the supported range are shown as plain text, and durations over 2^31 seconds do not wrap around.
- **Whole `float` values keep their exact value.** `123456792f` with `0` gave `123456790`.
- **Numbers round half away from zero, as Excel does.** `2.5` with `0` gave `2`, and `0.125` with `0.00` gave `0.12`. The value is reduced to 15 significant digits and rounded half up, so `2.675` with `0.00` gives `2.68` ([#1](https://github.com/XLibur/ExcelNumberFormat/issues/1)).
- **Literal text in a text section is kept when the format has fewer than four sections.** `"Prefix: "@` formatted `abc` as `abc`; it now gives `Prefix: abc` ([#1](https://github.com/XLibur/ExcelNumberFormat/issues/1)).
- **`.0`, `.00` and `.000` show that many fractional second digits and round.** They always showed three digits and never rounded. `mm:ss.0` of 12.34 seconds gave `00:12.340`; it now gives `00:12.3`, and 59.96 seconds gives `01:00.0`. Excel rounds the exact stored value half up, so does the library; the expected values were checked against Excel 16 ([#8](https://github.com/XLibur/ExcelNumberFormat/issues/8)).

### ⚡ Performance

- **Date and time formats are about 40% faster.** The tokens of a date format are classified once when it is parsed, instead of being compared as text on every call. `yyyy-mm-dd hh:mm:ss` takes about 690 ns instead of 1180 ns and allocates 96 B instead of 712 B. Parsing a date format costs a little more, which is repaid after a call or two on the same `NumberFormat`.
- **Formatting allocates 35% to 90% less.** The result builder is reused for each thread, the number string is no longer split into an array, padded numbers and fractional seconds are written without temporary strings, and typed values are no longer boxed inside the formatter.
- **Rounding takes a shortcut away from ties.** The value is formatted directly unless it lies within 1e-6 of a rounding tie, where the exact 15 digit rounding still applies. The result is the same: a run over 300,000 values, ties included, found no difference on .NET 8, 10 and 11.
- **Parsing is 10% to 25% faster.** Token classification is a `switch` on the first character instead of a series of string comparisons, character sets use `SearchValues<char>`, single character tokens are shared, and culture-sensitive string comparisons are now ordinal.
