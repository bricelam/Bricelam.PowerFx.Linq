# Unary operators

Power Fx | .NET
--- | ---
!x | !x
-x | -x
x% | x / 100.0

# Binary operators

Power Fx | .NET
--- | ---
x \|\| y | x \|\| y
x && y | x && y
x * y | x * y
x / y | x / y
x = y | x == y
x <> y | x != y
x < y | x < y
x <= y | x <= y
x > y | x > y
x >= y | x >= y
x + y | x + y
x - y | x - y
x & y | x + y (string)
x ^ y | Math.Pow
x in y | y.Contains(x)
x exactin y | y.Contains(x)


# Functions

Power Fx | .NET
--- | ---
Abs | Math.Abs
Acos | Math.ACos
Acot
AddColumns
AmPm
AmPmShort
And | x && y && ...
Asin | Math.Asin
AsType
Atan | Math.Atan
Atan2 | Math.Atan2
Average
Blank | null
Boolean
Char | ((char)value).ToString()
IsClock24
Coalesce | x ?? y ?? ...
ColorFade
ColorValue
Concat
Concatenate | string.Concat
Cos | Math.Cos
Cot | 1.0 / Math.Tan
Count
CountA
CountIf
CountRows
Date
DateAdd
DateDiff
DateTime
DateTimeValue | DateTime.Parse
DateValue
Day | dateTime.Day
Dec2Hex
Degrees | double.RadiansToDegrees
DropColumns
EDate
EOMonth
EncodeUrl
EndsWith | stringValue.EndsWith
Error
Exp | Math.Exp
Filter
Find
First
FirstN
ForAll
GUID | Guid.NewGuid or Guid.Parse
Hex2Dec
Hour | dateTime.Hour
If | x ? y : z
IfError
Index
Int | Math.Floor
IsBlank | x == null
IsBlankOrError
IsEmpty
IsError
IsNumeric
ISOWeekNum
IsToday
Language
Last
LastN
Left | stringValue.Substring(0, x)
Len
Ln | Math.Log
Log | Math.Log10 or Math.Log
LookUp
Lower | stringValue.ToLower
Max | Math.Max
Mid | stringValue.Substring(x - 1, y)
Min | Math.Min
Minute | dateTime.Minute
Mod | x % y
Month | dateTime.Month
MonthsLong
MonthsShort
Not | !value
Now
Or | x \|\| y \|\| ...
ParseJSON
Pi | Math.PI (constant)
PlainText
Power | Math.Pow
Proper
Radians | double.DegreesToRadians
Rand
RandBetween
Refresh
RenameColumns
Replace
RGBA
Right | stringValue.Substring(stringValue.Length - x)
Round | Math.Round
RoundDown | Math.Floor
RoundUp | Math.Ceiling
Search
Second | dateTime.Second
Sequence
ShowColumns
Shuffle
Sin | Math.Sin
Sort
SortByColumns
Split
Sqrt | Math.Sqrt
StartsWith | stringValue.StartsWith
StdevP
Substitute | string.Replace
Sum | x + y + ...
Switch
Table
Tan | Math.Tan
Text | obj.ToString
Time
TimeValue
TimeZoneOffset
Today
Trace
Trim
TrimEnds | stringValue.Trim
Trunc | Math.Truncate
Upper | stringValue.ToUpper
UTCNow | DateTime.UtcNow
UTCToday | DateTime.UtcNow.Date
Value | decimal.Parse
VarP
Weekday | dateTime.DayOfWeek
WeekdaysLong
WeekdaysShort
WeekNum
With
Year | dateTime.Year

# String interpolation

string.Concat

# Records

Dictionary<string, object?>

# Tables

List<Dictionary<string, object?>>
