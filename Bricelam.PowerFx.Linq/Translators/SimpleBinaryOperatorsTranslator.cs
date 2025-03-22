using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleBinaryOperatorsTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> _map = new()
    {
        { "And", Expression.AndAlso },

        // TODO: Handle additional arguments
        { "Coalesce", Expression.Coalesce },

        // TODO: Type lifting
        { "Mod", Expression.Modulo },

        { "Or", Expression.OrElse },

        // TODO: Handle aggregate
        { "Sum", Expression.Add }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
    {
        if (_map.TryGetValue(functionName, out var binaryExpressionFactory))
        {
            return CreateBinaryTree(binaryExpressionFactory, arguments);
        }

        return null;
    }

    static Expression CreateBinaryTree(
        Func<Expression, Expression, BinaryExpression> binaryExpressionFactory,
        IEnumerable<Expression> nodes)
    {
        Expression? tree = null;
        foreach (var node in nodes)
        {
            tree = tree is not null
                ? binaryExpressionFactory(tree, node)
                : node;
        }

        return tree
            ?? throw new ArgumentException("The value cannot be empty.", nameof(nodes));
    }
}
