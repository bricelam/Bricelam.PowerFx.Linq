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

Power Fx       | .NET
-------------- | ----
Abs            | Math.Abs
Acos           | Math.ACos
Acot           | (Math.Pi / 2.0) - Math.Atan
And            | x && y && ...
Asin           | Math.Asin
Atan           | Math.Atan
Atan2          | Math.Atan2
Average        | (x + y + ...) / n
Blank          | null
Char           | ((char)value).ToString()
Coalesce       | x ?? y ?? ...
Concatenate    | string.Concat
Cos            | Math.Cos
Cot            | 1.0 / Math.Tan
DateTimeValue  | DateTime.Parse
Day            | dateTime.Day
Degrees        | double.RadiansToDegrees
EndsWith       | stringValue.EndsWith
Exp            | Math.Exp
GUID           | Guid.NewGuid or Guid.Parse
Hour           | dateTime.Hour
If             | x ? y : z
Int            | Math.Floor
IsBlank        | x == null
Left           | stringValue.Substring(0, x)
Len            | stringValue.Length
Ln             | Math.Log
Log            | Math.Log10 or Math.Log
Lower          | stringValue.ToLower
Max            | Math.Max
Mid            | stringValue.Substring(x - 1, y)
Min            | Math.Min
Minute         | dateTime.Minute
Mod            | x % y
Month          | dateTime.Month
Not            | !value
Now            | DateTime.Now
Or             | x \|\| y \|\| ...
Pi             | Math.PI
Power          | Math.Pow
Radians        | double.DegreesToRadians
Right          | stringValue.Substring(stringValue.Length - x)
Round          | Math.Round
RoundDown      | Math.Floor
RoundUp        | Math.Ceiling
Second         | dateTime.Second
Sin            | Math.Sin
Split          | stringValue.Split
Sqrt           | Math.Sqrt
StartsWith     | stringValue.StartsWith
Substitute     | string.Replace
Sum            | x + y + ...
Tan            | Math.Tan
Text           | obj.ToString
Today          | DateTime.Today
TrimEnds       | stringValue.Trim
Trunc          | Math.Truncate
Upper          | stringValue.ToUpper
UTCNow         | DateTime.UtcNow
UTCToday       | DateTime.UtcNow.Date
Value          | decimal.Parse
Weekday        | dateTime.DayOfWeek
Year           | dateTime.Year

## String interpolation

string.Concat

## Records

Dictionary<string, object?>

## Tables

List<Dictionary<string, object?>>

## See also

- [Operators and Identifiers](https://learn.microsoft.com/power-platform/power-fx/operators)
- [Power Fx formula reference overview](https://learn.microsoft.com/power-platform/power-fx/formula-reference-overview)
