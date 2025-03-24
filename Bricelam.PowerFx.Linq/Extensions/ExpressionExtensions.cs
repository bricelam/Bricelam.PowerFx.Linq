namespace System.Linq.Expressions;

static class ExpressionExtensions
{
    static readonly Dictionary<Type, int> _typePrecindence = new()
    {
        { typeof(double), 0 },
        { typeof(double?), 1 },
        { typeof(decimal), 2 },
        { typeof(decimal?), 3 }
    };

    public static Expression ConvertIfNeeded(Expression expression, Type type)
        => expression.Type == type
            ? expression
            : Expression.Convert(expression, type);

    public static BinaryExpression LiftAndAdd(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.Add(left, right);
    }

    public static BinaryExpression LiftAndSubtract(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.Subtract(left, right);
    }

    public static BinaryExpression LiftAndMultiply(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.Multiply(left, right);
    }

    public static BinaryExpression LiftAndDivide(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.Divide(left, right);
    }

    private static (Expression Left, Expression Right) Lift(Expression left, Expression right)
    {
        var leftType = left.Type;
        var rightType = right.Type;

        return leftType == rightType
            ? (left, right)
            : _typePrecindence[leftType] > _typePrecindence[rightType]
                ? (left, Expression.Convert(right, leftType))
                : (Expression.Convert(left, rightType), right);
    }
}
