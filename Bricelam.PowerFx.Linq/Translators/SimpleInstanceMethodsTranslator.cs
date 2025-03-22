using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleInstanceMethodsTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, (Type Type, string MethodName)> _map = new()
    {
        { "EndsWith", (typeof(string), nameof(string.EndsWith)) },
        { "Lower", (typeof(string), nameof(string.ToLower)) },
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
            var method = mapping.Type.GetMethod(mapping.MethodName, arguments.Skip(1).Select(a => a.Type).ToArray());
            if (method is not null)
            {
                return Expression.Call(arguments[0], method, arguments.Skip(1));
            }
        }

        return null;
    }
}
