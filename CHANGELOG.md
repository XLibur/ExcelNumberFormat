# Changelog

## Contents

- [Unreleased](#unreleased)

## Unreleased

### 💥 Breaking Changes

- **The package, assembly and namespace are renamed to `Xlibur.ExcelNumberFormat`.** Change the package reference and `using ExcelNumberFormat;` to `using Xlibur.ExcelNumberFormat;`.
- **The library targets `net8.0`, `net10.0` and `net11.0`.** `net20`, `netstandard1.0` and `netstandard2.0` are no longer built.

### ✨ New Features

- **`byte`, `sbyte`, `ushort`, `uint`, `long`, `ulong`, `float`, `decimal`, `DateOnly` and `TimeOnly` values are formatted.** Only `double`, `int` and `short` were treated as numbers, so any other type came back as plain text. `float` goes through its shortest round-trip string, so `1234.56f` formats as `1234.56` ([#5](https://github.com/XLibur/ExcelNumberFormat/issues/5)).
- **`[$-F800]` and `[$-F400]` format with the culture's long date and long time pattern.** Excel ignores the rest of the format for these system codes, and so does the library now ([#3](https://github.com/XLibur/ExcelNumberFormat/issues/3)).

### 🐛 Bug Fixes

- **Numbers round half away from zero, as Excel does.** `2.5` with `0` gave `2`, and `0.125` with `0.00` gave `0.12`. The value is reduced to 15 significant digits and rounded half up, so `2.675` with `0.00` gives `2.68` ([#1](https://github.com/XLibur/ExcelNumberFormat/issues/1)).
- **Literal text in a text section is kept when the format has fewer than four sections.** `"Prefix: "@` formatted `abc` as `abc`; it now gives `Prefix: abc` ([#1](https://github.com/XLibur/ExcelNumberFormat/issues/1)).
- **`.0`, `.00` and `.000` show that many fractional second digits and round.** They always showed three digits and never rounded. `mm:ss.0` of 12.34 seconds gave `00:12.340`; it now gives `00:12.3`, and 59.96 seconds gives `01:00.0`. Excel rounds the exact stored value half up, so does the library; the expected values were checked against Excel 16 ([#8](https://github.com/XLibur/ExcelNumberFormat/issues/8)).
