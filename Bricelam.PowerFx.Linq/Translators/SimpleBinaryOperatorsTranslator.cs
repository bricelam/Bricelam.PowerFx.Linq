using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleBinaryOperatorsTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> _map = new()
    {
        { "And", Expression.AndAlso },
        { "Coalesce", Expression.Coalesce },
        { "Mod", ExpressionExtensions.LiftAndModulo },
        { "Or", Expression.OrElse },

        // TODO: Handle aggregate
        { "Sum", ExpressionExtensions.LiftAndAdd }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
    {
        if (_map.TryGetValue(functionName, out var binaryExpressionFactory))
        {
            return CreateBinaryTree(binaryExpressionFactory, arguments);
        }

        return null;
    }

    public static Expression CreateBinaryTree(
        Func<Expression, Expression, BinaryExpression> binaryExpressionFactory,
        IEnumerable<Expression> operands)
    {
        Expression? tree = null;
        foreach (var operand in operands)
        {
            tree = tree is not null
                ? binaryExpressionFactory(tree, operand)
                : operand;
        }

        return tree
            ?? throw new ArgumentException("The value cannot be empty.", nameof(operands));
    }
}
