# Bricelam.PowerFx.Linq

![build status](https://img.shields.io/github/actions/workflow/status/bricelam/Bricelam.PowerFx.Linq/dotnet.yml?main) ![latest version](https://img.shields.io/myget/bricelam/v/Bricelam.PowerFx.Linq) ![downloads](https://img.shields.io/myget/bricelam/dt/Bricelam.PowerFx.Linq) ![license](https://img.shields.io/badge/license-MS--PL-green)

Use Power Fx inside of LINQ.

This project translates Power Fx formulas into .NET Expression trees enabling you to use them inside of LINQ queries.

## Installation

The latest prerelease version is available on [MyGet](https://www.myget.org/feed/bricelam/package/nuget/Bricelam.PowerFx.Linq).

```sh
dotnet add package Bricelam.PowerFx.Linq --source https://www.myget.org/F/bricelam/api/v3/index.json
```

## Usage

The `PowerFxExpression` class allows you to create lambda expressions from formulas.

```cs
// TIP: Hardcoded formulas aren't very interesting, but imagine if they're
//      defined outside the code!
var formula = "Radius = 1";

var predicate = PowerFxExpression.Predicate<Circle>(formula);
var query = circles.Where(predicate);
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

- [Bricelam.PowerFx.Linq Docs](https://www.bricelam.net/Bricelam.PowerFx.Linq)
- [Microsoft Power Fx overview](https://learn.microsoft.com/power-platform/power-fx/overview)
