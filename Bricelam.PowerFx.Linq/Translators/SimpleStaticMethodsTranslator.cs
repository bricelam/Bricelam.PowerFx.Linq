using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleStaticMethodsTranslator : IFunctionCallTranslator
{
    // TODO: Handle aggregates
    static readonly Dictionary<string, (Type Type, string MethodName)> _map = new()
    {
        { "Abs", (typeof(Math), nameof(Math.Abs)) },
        { "Acos", (typeof(Math), nameof(Math.Acos)) },
        { "Asin", (typeof(Math), nameof(Math.Asin)) },
        { "Atan", (typeof(Math), nameof(Math.Atan)) },

        // TODO: Handle more than three parameters; handle non-strings
        { "Concatenate", (typeof(string), nameof(string.Concat)) },

        { "Cos", (typeof(Math), nameof(Math.Cos)) },
        { "DateTimeValue", (typeof(DateTime), nameof(DateTime.Parse)) },
        { "Degrees", (typeof(double), nameof(double.RadiansToDegrees)) },
        { "Exp", (typeof(Math), nameof(Math.Exp)) },
        { "Int", (typeof(Math), nameof(Math.Floor)) },
        { "Ln", (typeof(Math), nameof(Math.Log)) },

        // TODO: Handle more than two parameters
        { "Max", (typeof(Math), nameof(Math.Max)) },
        { "Min", (typeof(Math), nameof(Math.Min)) },

        { "Power", (typeof(Math), nameof(Math.Pow)) },
        { "Radians", (typeof(double), nameof(double.DegreesToRadians)) },
        { "Round", (typeof(Math), nameof(Math.Round)) },
        { "Sin", (typeof(Math), nameof(Math.Sin)) },
        { "Sqrt", (typeof(Math), nameof(Math.Sqrt)) },
        { "Tan", (typeof(Math), nameof(Math.Tan)) },
        { "Trunc", (typeof(Math), nameof(Math.Truncate)) },

        // TODO: Handle LanguageTag parameter; honor NumberIsFloat
        { "Value", (typeof(double), nameof(double.Parse)) }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
    {
        if (_map.TryGetValue(functionName, out var mapping))
        {
            var overloads = mapping.Type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == mapping.MethodName);

            return ExpressionExtensions.CallBestOverload(overloads, arguments);
        }

        return null;
    }
}
