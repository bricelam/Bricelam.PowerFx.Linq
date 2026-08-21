using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleBinaryOperatorsTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, Func<Expression, Expression, IPowerFxTranslatorContext, BinaryExpression>> _map = new()
    {
        { "And", (l, r, c) => Expression.AndAlso(l, r) },
        { "Coalesce", (l, r, c) => ExpressionExtensions.LiftAndCoalesce(l, r) },
        { "Mod", (l, r, c) => ExpressionExtensions.LiftAndModulo(l, r, c.NumberIsDecimal ? typeof(decimal?) : typeof(float?)) },
        { "Or", (l, r, c) => Expression.OrElse(l, r) },

        // TODO: Handle aggregate
        { "Sum", (l, r, c) => ExpressionExtensions.LiftAndAdd(l, r, c.NumberIsDecimal ? typeof(decimal?) : typeof(float?)) }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments, IPowerFxTranslatorContext context)
    {
        if (_map.TryGetValue(functionName, out var binaryExpressionFactory))
        {
            return CreateBinaryTree(binaryExpressionFactory, arguments, context);
        }

        return null;
    }

    public static Expression CreateBinaryTree(
        Func<Expression, Expression, IPowerFxTranslatorContext, BinaryExpression> binaryExpressionFactory,
        IEnumerable<Expression> operands,
        IPowerFxTranslatorContext context)
    {
        Expression? tree = null;
        foreach (var operand in operands)
        {
            tree = tree is not null
                ? binaryExpressionFactory(tree, operand, context)
                : operand;
        }

        return tree
            ?? throw new ArgumentException("The value cannot be empty.", nameof(operands));
    }
}
