# Bricelam.PowerFx.Linq

![build status](https://img.shields.io/github/actions/workflow/status/bricelam/Bricelam.PowerFx.Linq/dotnet.yml?main) ![latest version](https://img.shields.io/nuget/v/Bricelam.PowerFx.Linq) ![downloads](https://img.shields.io/nuget/dt/Bricelam.PowerFx.Linq) ![license](https://img.shields.io/badge/license-MS--PL-green)

Use Power Fx inside of LINQ.

This project translates Power Fx formulas into .NET Expression trees enabling you to use them inside of LINQ queries.

## Installation

The latest prerelease version is available on [NuGet](https://www.nuget.org/packages/Bricelam.PowerFx.Linq).

```sh
dotnet add package Bricelam.PowerFx.Linq
```

## Usage

The `PowerFxExpression` class allows you to create lambda expressions from formulas.

```cs
// TIP: Hardcoded formulas aren't very interesting, but imagine if they're
//      defined outside the code!
var formula = "Radius = 1";

var predicate = PowerFxExpression.Predicate<Circle>(formula);
var unitCircles = circles.Where(predicate);
```

The `PowerFxQueryable` class provides queryable extension methods that use formulas.

```cs
var query = circles
    .AddColumns(
        ("Diameter", "2 * Radius"),
        ("Circumfrence", "2 * Pi() * Radius")
        ("Area", "Pi() * Radius^2"));
```

## See also

- [Supported Translations](docs/translations.md)
- [Microsoft Power Fx overview](https://learn.microsoft.com/power-platform/power-fx/overview)
