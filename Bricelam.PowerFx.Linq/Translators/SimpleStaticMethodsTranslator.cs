using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleStaticMethodsTranslator : IFunctionCallTranslator
{
    // TODO: Handle aggregates
    static readonly Dictionary<string, MethodInfo[]> _map = new()
    {
        {
            "Abs",
            [
                ..typeof(Math).GetMethods(nameof(Math.Abs)),
                typeof(MathF).GetMethod(nameof(MathF.Abs))!
            ]
        },
        {
            "Acos",
            [
                typeof(Math).GetMethod(nameof(Math.Acos))!,
                typeof(MathF).GetMethod(nameof(MathF.Acos))!
            ]
        },
        {
            "Asin",
            [
                typeof(Math).GetMethod(nameof(Math.Asin))!,
                typeof(MathF).GetMethod(nameof(MathF.Asin))!
            ]
        },
        {
            "Atan",
            [
                typeof(Math).GetMethod(nameof(Math.Atan))!,
                typeof(MathF).GetMethod(nameof(MathF.Atan))!
            ]
        },
        {
            // TODO: Handle more than four parameters
            "Concatenate",
            [
                typeof(string).GetMethod(nameof(string.Concat), [typeof(string), typeof(string)])!,
                typeof(string).GetMethod(nameof(string.Concat), [typeof(string), typeof(string), typeof(string)])!,
                typeof(string).GetMethod(nameof(string.Concat), [typeof(string), typeof(string), typeof(string), typeof(string)])!
            ]
        },
        {
            "Cos",
            [
                typeof(Math).GetMethod(nameof(Math.Cos))!,
                typeof(MathF).GetMethod(nameof(MathF.Cos))!
            ]
        },
        {
            "DateTimeValue",
            [typeof(DateTime).GetMethod(nameof(DateTime.Parse), [typeof(string)])!]
        },
        {
            "Degrees",
            [
                typeof(double).GetMethod(nameof(double.RadiansToDegrees))!,
                typeof(float).GetMethod(nameof(float.RadiansToDegrees))!
            ]
        },
        {
            "Exp",
            [
                typeof(Math).GetMethod(nameof(Math.Exp))!,
                typeof(MathF).GetMethod(nameof(MathF.Exp))!
            ]
        },
        {
            "GUID",
            [
                typeof(Guid).GetMethod(nameof(Guid.NewGuid), Type.EmptyTypes)!,
                typeof(Guid).GetMethod(nameof(Guid.Parse), [typeof(string)])!
            ]
        },
        {
            "Int",
            [
                ..typeof(Math).GetMethods(nameof(Math.Floor)),
                typeof(MathF).GetMethod(nameof(MathF.Floor))!
            ]
        },
        {
            "Ln",
            [
                typeof(Math).GetMethod(nameof(Math.Log), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Log), [typeof(float)])!
            ]
        },
        {
            "Log",
            [
                typeof(Math).GetMethod(nameof(Math.Log10), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Log10), [typeof(float)])!,
                typeof(Math).GetMethod(nameof(Math.Log), [typeof(double), typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Log), [typeof(float), typeof(float)])!
            ]
        },
        {
            // TODO: Handle more than two parameters
            "Max",
            typeof(Math).GetMethods(nameof(Math.Max)).ToArray()
        },
        {
            // TODO: Handle more than two parameters
            "Min",
            typeof(Math).GetMethods(nameof(Math.Min)).ToArray()
        },
        {
            "Power",
            [
                typeof(Math).GetMethod(nameof(Math.Pow))!,
                typeof(MathF).GetMethod(nameof(MathF.Pow))!
            ]
        },
        {
            "Radians",
            [
                typeof(double).GetMethod(nameof(double.DegreesToRadians))!,
                typeof(float).GetMethod(nameof(float.DegreesToRadians))!
            ]
        },
        {
            "Round",
            [
                typeof(Math).GetMethod(nameof(Math.Round), [typeof(double), typeof(int)])!,
                typeof(Math).GetMethod(nameof(Math.Round), [typeof(decimal), typeof(int)])!,
                typeof(MathF).GetMethod(nameof(MathF.Round), [typeof(float), typeof(int)])!
            ]
        },
        {
            "Sin",
            [
                typeof(Math).GetMethod(nameof(Math.Sin))!,
                typeof(MathF).GetMethod(nameof(MathF.Sin))!
            ]
        },
        {
            "Sqrt",
            [
                typeof(Math).GetMethod(nameof(Math.Sqrt))!,
                typeof(MathF).GetMethod(nameof(MathF.Sqrt))!
            ]
        },
        {
            "Tan",
            [
                typeof(Math).GetMethod(nameof(Math.Tan))!,
                typeof(MathF).GetMethod(nameof(MathF.Tan))!
            ]
        },
        {
            "Trunc",
            [
                ..typeof(Math).GetMethods(nameof(Math.Truncate)),
                typeof(MathF).GetMethod(nameof(MathF.Truncate))!
            ]
        }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
        => _map.TryGetValue(functionName, out var overloads)
            ? ExpressionExtensions.CallBestOverload(overloads, arguments)
            : null;
}
