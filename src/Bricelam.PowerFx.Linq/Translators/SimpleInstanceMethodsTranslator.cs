using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleInstanceMethodsTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, MethodInfo> _map = new()
    {
        { "EndsWith", typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])! },
        { "Lower", typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)! },
        { "StartsWith", typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])! },

        // TODO: Handle InstanceNumber parameter
        { "Substitute", typeof(string).GetMethod(nameof(string.Replace), [typeof(string), typeof(string)])! },

        // TODO: Does this need a convert?
        // TODO: Handle format parameters
        { "Text", typeof(object).GetMethod(nameof(ToString), Type.EmptyTypes)! },

        { "TrimEnds", typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes)! },
        { "Upper", typeof(string).GetMethod(nameof(string.ToUpper), Type.EmptyTypes)! }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments, IPowerFxTranslatorContext context)
        => _map.TryGetValue(functionName, out var method)
            ? Expression.Call(arguments[0], method, arguments.Skip(1).ToArray())
            : null;
}
