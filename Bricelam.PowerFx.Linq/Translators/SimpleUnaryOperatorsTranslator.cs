using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleUnaryOperatorsTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, Func<Expression, UnaryExpression>> _map = new()
    {
        { "Not", Expression.Not }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
    {
        if (_map.TryGetValue(functionName, out var unaryExpressionFactory))
        {
            return unaryExpressionFactory(arguments[0]);
        }

        return null;
    }
}
