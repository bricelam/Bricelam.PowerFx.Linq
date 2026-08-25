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
                typeof(Math).GetMethod(nameof(Math.Abs), [typeof(short)])!,
                typeof(Math).GetMethod(nameof(Math.Abs), [typeof(int)])!,
                typeof(Math).GetMethod(nameof(Math.Abs), [typeof(long)])!,
                typeof(Math).GetMethod(nameof(Math.Abs), [typeof(float)])!,
                typeof(Math).GetMethod(nameof(Math.Abs), [typeof(double)])!,
                typeof(Math).GetMethod(nameof(Math.Abs), [typeof(decimal)])!
            ]
        },
        {
            "Acos",
            [
                typeof(Math).GetMethod(nameof(Math.Acos), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Acos), [typeof(float)])!
            ]
        },
        {
            "Asin",
            [
                typeof(Math).GetMethod(nameof(Math.Asin), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Asin), [typeof(float)])!
            ]
        },
        {
            "Atan",
            [
                typeof(Math).GetMethod(nameof(Math.Atan), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Atan), [typeof(float)])!
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
                typeof(Math).GetMethod(nameof(Math.Cos), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Cos), [typeof(float)])!
            ]
        },
        {
            "DateTimeValue",
            [typeof(DateTime).GetMethod(nameof(DateTime.Parse), [typeof(string)])!]
        },
        {
            "Degrees",
            [
                typeof(double).GetMethod(nameof(double.RadiansToDegrees), [typeof(double)])!,
                typeof(float).GetMethod(nameof(float.RadiansToDegrees), [typeof(float)])!
            ]
        },
        {
            "Exp",
            [
                typeof(Math).GetMethod(nameof(Math.Exp), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Exp), [typeof(float)])!
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
                typeof(Math).GetMethod(nameof(Math.Floor), [typeof(double)])!,
                typeof(Math).GetMethod(nameof(Math.Floor), [typeof(decimal)])!,
                typeof(MathF).GetMethod(nameof(MathF.Floor), [typeof(float)])!
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
            [
                typeof(Math).GetMethod(nameof(Math.Max), [typeof(byte), typeof(byte)])!,
                typeof(Math).GetMethod(nameof(Math.Max), [typeof(short), typeof(short)])!,
                typeof(Math).GetMethod(nameof(Math.Max), [typeof(int), typeof(int)])!,
                typeof(Math).GetMethod(nameof(Math.Max), [typeof(long), typeof(long)])!,
                typeof(Math).GetMethod(nameof(Math.Max), [typeof(float), typeof(float)])!,
                typeof(Math).GetMethod(nameof(Math.Max), [typeof(double), typeof(double)])!,
                typeof(Math).GetMethod(nameof(Math.Max), [typeof(decimal), typeof(decimal)])!,
            ]
        },
        {
            // TODO: Handle more than two parameters
            "Min",
            [
                typeof(Math).GetMethod(nameof(Math.Min), [typeof(byte), typeof(byte)])!,
                typeof(Math).GetMethod(nameof(Math.Min), [typeof(short), typeof(short)])!,
                typeof(Math).GetMethod(nameof(Math.Min), [typeof(int), typeof(int)])!,
                typeof(Math).GetMethod(nameof(Math.Min), [typeof(long), typeof(long)])!,
                typeof(Math).GetMethod(nameof(Math.Min), [typeof(float), typeof(float)])!,
                typeof(Math).GetMethod(nameof(Math.Min), [typeof(double), typeof(double)])!,
                typeof(Math).GetMethod(nameof(Math.Min), [typeof(decimal), typeof(decimal)])!,
            ]
        },
        {
            "Power",
            [
                typeof(Math).GetMethod(nameof(Math.Pow), [typeof(double), typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Pow), [typeof(float), typeof(float)])!
            ]
        },
        {
            "Radians",
            [
                typeof(double).GetMethod(nameof(double.DegreesToRadians), [typeof(double)])!,
                typeof(float).GetMethod(nameof(float.DegreesToRadians), [typeof(float)])!
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
                typeof(Math).GetMethod(nameof(Math.Sin), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Sin), [typeof(float)])!
            ]
        },
        {
            "Sqrt",
            [
                typeof(Math).GetMethod(nameof(Math.Sqrt), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Sqrt), [typeof(float)])!
            ]
        },
        {
            "Tan",
            [
                typeof(Math).GetMethod(nameof(Math.Tan), [typeof(double)])!,
                typeof(MathF).GetMethod(nameof(MathF.Tan), [typeof(float)])!
            ]
        },
        {
            "Trunc",
            [
                typeof(Math).GetMethod(nameof(Math.Truncate), [typeof(double)])!,
                typeof(Math).GetMethod(nameof(Math.Truncate), [typeof(decimal)])!,
                typeof(MathF).GetMethod(nameof(MathF.Truncate), [typeof(float)])!
            ]
        }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments, IPowerFxTranslatorContext context)
        => _map.TryGetValue(functionName, out var overloads)
            ? ExpressionExtensions.CallBestOverload(overloads, arguments)
            : null;
}
