using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleConstantsTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, object?> _map = new()
    {
        { "Blank", null },

        // NB: Translated to constant (and not property) to match C# compiler
        { "Pi", Math.PI }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
    {
        if (_map.TryGetValue(functionName, out var value))
        {
            return Expression.Constant(value);
        }

        return null;
    }
}
