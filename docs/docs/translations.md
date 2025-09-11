# Supported Translations

These are the translations provided by this library.

## Unary operators

Power Fx | .NET
-------- | ----
!x       | !x
-x       | -x
x%       | x / 100.0

## Binary operators

Power Fx    | .NET
----------- | ----
x \|\| y    | x \|\| y
x && y      | x && y
x * y       | x * y
x / y       | x / y
x = y       | x == y
x <> y      | x != y
x < y       | x < y
x <= y      | x <= y
x > y       | x > y
x >= y      | x >= y
x + y       | x + y
x - y       | x - y
x & y       | x + y
x ^ y       | Math.Pow(x, y)
x in y      | y.Contains(x)
x exactin y | y.Contains(x)

## Functions

Power Fx                      | .NET
----------------------------- | ----
Abs(x)                        | Math.Abs(x)
Acos(x)                       | Math.ACos(x)
Acot(x)                       | (Math.Pi / 2.0) - Math.Atan(x)
And(x, y, ...)                | x && y && ...
Asin(x)                       | Math.Asin(x)
Atan(x)                       | Math.Atan(x)
Atan2(x, y)                   | Math.Atan2(y, x)
Average(x, y, ...)            | (x + y + ...) / n
Blank()                       | null
Char(x)                       | ((char)x).ToString()
Coalesce(x, y, ...)           | x ?? y ?? ...
Concatenate(x, y, ...)        | string.Concat(x, y, ...)
Cos(x)                        | Math.Cos(x)
Cot(x)                        | 1.0 / Math.Tan(x)
DateTimeValue(x)              | DateTime.Parse(x)
Day(dateTime)                 | dateTime.Day
Degrees(x)                    | double.RadiansToDegrees(x)
EndsWith(stringValue, x)      | stringValue.EndsWith(x)
Exp(x)                        | Math.Exp(x)
GUID()                        | Guid.NewGuid()
GUID(x)                       | Guid.Parse(x)
Hour(dateTime)                | dateTime.Hour
If(x, y, z)                   | x ? y : z
Int(x)                        | Math.Floor(x)
IsBlank(x)                    | x == null
Left(stringValue, x)          | stringValue.Substring(0, x)
Len(stringValue)              | stringValue.Length
Ln(x)                         | Math.Log(x)
Log(x)                        | Math.Log10(x)
Log(x, y)                     | Math.Log(x, y)
Lower(stringValue)            | stringValue.ToLower()
Max(x, y)                     | Math.Max(x, y)
Mid(stringValue, x, y)        | stringValue.Substring(x - 1, y)
Min(x, y)                     | Math.Min(x, y)
Minute(dateTime)              | dateTime.Minute
Mod(x, y)                     | x % y
Month(dateTime)               | dateTime.Month
Not(x)                        | !x
Now()                         | DateTime.Now
Or(x, y, ...)                 | x \|\| y \|\| ...
Pi()                          | Math.PI
Power(x, y)                   | Math.Pow(x, y)
Radians(x)                    | double.DegreesToRadians(x)
Right(stringValue, x)         | stringValue.Substring(stringValue.Length - x)
Round(x, y)                   | Math.Round(x, y)
Second(dateTime)              | dateTime.Second
Sin(x)                        | Math.Sin(x)
Split(stringValue, x)         | stringValue.Split(x)
Sqrt(x)                       | Math.Sqrt(x)
StartsWith(stringValue, x)    | stringValue.StartsWith(x)
Substitute(stringValue, x, y) | stringValue.Replace(x, y)
Sum(x, y, ...)                | x + y + ...
Tan(x)                        | Math.Tan(x)
Text(obj)                     | obj.ToString()
Today()                       | DateTime.Today
TrimEnds(stringValue)         | stringValue.Trim()
Trunc(x)                      | Math.Truncate(x)
Upper(stringValue)            | stringValue.ToUpper()
UTCNow()                      | DateTime.UtcNow
UTCToday()                    | DateTime.UtcNow.Date
Value(x)                      | double.Parse(x)
Weekday(dateTime)             | dateTime.DayOfWeek
Year(dateTime)                | dateTime.Year

## String interpolation

String interpolation in Power Fx is translated to string concatenation in .NET.

**Power Fx**

```
$"hello {x}"
```

**.NET**

```cs
"hello " + x
```

## Records

Records in Power Fx are translated to dictionaries in .NET.

**Power Fx**

```
{
    Value: x
}
```

**.NET**

```cs
new Dictionary<string, object?>
{
    { "Value", x }
}
```

## Tables

Tables in Power Fx are translated to lists in .NET. The following example creates a single-column table using the inline value table syntax.

**Power Fx**

```
[
    x,
    y
]
```

**.NET**

```cs
new List<Dictionary<string, object?>>
{
    new() { { "Value", x } },
    new() { { "Value", y } },
}
```

## See also

- [Operators and Identifiers](https://learn.microsoft.com/power-platform/power-fx/operators)
- [Power Fx formula reference overview](https://learn.microsoft.com/power-platform/power-fx/formula-reference-overview)
