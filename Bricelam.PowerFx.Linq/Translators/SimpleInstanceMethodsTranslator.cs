using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleInstanceMethodsTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, (Type Type, string MethodName)> _map = new()
    {
        { "EndsWith", (typeof(string), nameof(string.EndsWith)) },
        { "Lower", (typeof(string), nameof(string.ToLower)) },

        // TODO: Handle multiple separators
        { "Split", (typeof(string), nameof(string.Split)) },

        { "StartsWith", (typeof(string), nameof(string.StartsWith)) },

        // TODO: Handle InstanceNumber parameter
        { "Substitute", (typeof(string), nameof(string.Replace)) },

        // TODO: Handle format parameters
        { "Text", (typeof(object), nameof(ToString)) },

        { "TrimEnds", (typeof(string), nameof(string.Trim)) },
        { "Upper", (typeof(string), nameof(string.ToUpper)) }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
    {
        if (_map.TryGetValue(functionName, out var mapping))
        {
            var overloads = mapping.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.Name == mapping.MethodName);

            return ExpressionExtensions.CallBestOverload(arguments[0], overloads, arguments.Skip(1));
        }

        return null;
    }
}
