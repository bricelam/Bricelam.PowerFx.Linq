#pragma warning disable IDE0130

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Bricelam.PowerFx.Linq;

namespace System.Linq.Expressions;

static class ExpressionExtensions
{
    static readonly List<Type> _types =
    [
        typeof(byte),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(float),
        typeof(double),
        typeof(decimal)
    ];

    static readonly int[,] _conversions = new[,]
    {
        // byte
        // |   short
        // |   |   int
        // |   |   |   long
        // |   |   |   |   float
        // |   |   |   |   |  double
        // |   |   |   |   |  |  decimal
        {  0,  1,  2,  3,  4, 5, 6 }, // byte
        {  9,  0,  1,  2,  3, 4, 5 }, // short
        { 10,  9,  0,  1,  7, 2, 3 }, // int
        { 11, 10,  9,  0,  8, 7, 3 }, // long
        { 15, 14, 13, 12,  0, 1, 2 }, // float
        { 15, 14, 13, 12,  9, 0, 1 }, // double
        { 15, 14, 13, 12, 10, 9, 0 }  // decimal
    };

    public static Expression CallBestOverload(IEnumerable<MethodInfo> overloads, IEnumerable<Expression> arguments)
        => CallBestOverload(instance: null, overloads, arguments);

    public static Expression CallBestOverload(Expression? instance, IEnumerable<MethodInfo> overloads, IEnumerable<Expression> arguments)
    {
        var argumentsList = arguments.ToList();

        var overloadsMap = overloads
            .Where(o => o.GetParameters().Length == argumentsList.Count)
            .ToDictionary<MethodInfo, IReadOnlyList<Type>>(
                o => o.GetParameters().Select(p => p.ParameterType).ToList(),
                new SequenceEqualComparer<Type>());
        if (overloadsMap.Count == 0)
        {
            var method = overloadsMap.Values.First();

            throw new PowerFxLinqException($"No overload of '{method.DeclaringType!.Name}.{method.Name}' takes {argumentsList.Count} arguments.");
        }

        // Try for an exact match
        if (overloadsMap.TryGetValue(argumentsList.Select(a => a.Type).ToList(), out var match))
        {
            return Expression.Call(instance, match, argumentsList);
        }

        MethodInfo? bestOverload = null;
        IReadOnlyList<Type>? bestOverloadParameters = null;
        var bestOverloadDistance = 0;
        foreach (var overload in overloadsMap)
        {
            var distance = 0;
            for (var i = 0; i < argumentsList.Count; i++)
            {
                var argumentType = argumentsList[i].Type;
                var fromNullable = argumentType.IsNullable();
                if (fromNullable)
                {
                    argumentType = argumentType.GenericTypeArguments[0];
                }

                var parameterType = overload.Key[i];
                var toNullable = parameterType.IsNullable();
                if (toNullable)
                {
                    parameterType = parameterType.GenericTypeArguments[0];
                }

                var argumentIndex = _types.IndexOf(argumentType);
                var parameterIndex = _types.IndexOf(parameterType);

                // TODO: Add nullable conversion cost
                distance += _conversions[argumentIndex, parameterIndex];
            }

            if (bestOverload is null
                || bestOverloadDistance > distance)
            {
                bestOverload = overload.Value;
                bestOverloadParameters = overload.Key;
                bestOverloadDistance = distance;
            }
        }

        Debug.Assert(bestOverload is not null);
        Debug.Assert(bestOverloadParameters is not null);

        return Expression.Call(
            instance,
            bestOverload,
            argumentsList
                .Select((a, i) => ConvertIfNeeded(a, bestOverloadParameters[i]))
                .ToArray());
    }

    public static Expression ConvertIfNeeded(Expression expression, Type type)
    {
        if (expression.Type == type)
            return expression;

        if (expression is ConstantExpression constantExpression)
        {
            var value = constantExpression.Value;
            if (value is not null)
            {
                var nullable = type.IsNullable();
                var conversionType = nullable
                    ? type.GenericTypeArguments[0]
                    : type;
                value = Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
                if (nullable)
                {
                    value = type.GetConstructor([conversionType])!.Invoke([value]);
                }
            }

            return Expression.Constant(value, type);
        }

        return Expression.Convert(expression, type);
    }

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

    public static BinaryExpression LiftAndModulo(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.Modulo(left, right);
    }

    public static BinaryExpression LiftAndCoalesce(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.Coalesce(left, right);
    }

    public static BinaryExpression LiftAndEqual(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.Equal(left, right);
    }

    public static BinaryExpression LiftAndNotEqual(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.NotEqual(left, right);
    }

    public static BinaryExpression LiftAndLessThan(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.LessThan(left, right);
    }

    public static BinaryExpression LiftAndLessThanOrEqual(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.LessThanOrEqual(left, right);
    }

    public static BinaryExpression LiftAndGreaterThan(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.GreaterThan(left, right);
    }

    public static BinaryExpression LiftAndGreaterThanOrEqual(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.GreaterThanOrEqual(left, right);
    }

    // TODO: Can we share logic with CallBestOverload?
    static (Expression Left, Expression Right) Lift(Expression left, Expression right)
    {
        var leftType = left.Type;
        var rightType = right.Type;
        if (leftType == rightType)
            return (left, right);

        var nullable = false;
        if (leftType.IsNullable())
        {
            nullable = true;
            leftType = leftType.GenericTypeArguments[0];
        }
        if (rightType.IsNullable())
        {
            nullable = true;
            rightType = rightType.GenericTypeArguments[0];
        }

        Type liftedType;
        if (leftType == rightType)
        {
            liftedType = leftType;
        }
        else if (leftType == typeof(object))
        {
            nullable = true;
            liftedType = rightType;
        }
        else if (rightType == typeof(object))
        {
            nullable = true;
            liftedType = leftType;
        }
        else
        {
            var leftIndex = _types.IndexOf(leftType);
            var rightIndex = _types.IndexOf(rightType);
            if (leftIndex == -1 || rightIndex == -1)
            {
                throw new PowerFxLinqException($"Cannot lift operands of type '{left.Type}' and '{right.Type}'.");
            }

            liftedType = _conversions[leftIndex, rightIndex] < _conversions[rightIndex, leftIndex]
                ? rightType
                : leftType;
        }

        if (nullable)
        {
            liftedType = liftedType.AsNullable();
        }

        return (ConvertIfNeeded(left, liftedType), ConvertIfNeeded(right, liftedType));
    }

    class SequenceEqualComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
            => x is null
                ? y is null
                : y is not null && Enumerable.SequenceEqual(x, y);

        public int GetHashCode([DisallowNull] IEnumerable<T> obj)
        {
            var result = 0;

            if (obj is not null)
            {
                foreach (var item in obj)
                {
                    if (item is not null)
                    {
                        result ^= item.GetHashCode();
                    }
                }
            }

            return result;
        }
    }
}
