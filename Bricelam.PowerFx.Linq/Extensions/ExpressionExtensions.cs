#pragma warning disable IDE0130

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Bricelam.PowerFx.Linq;

namespace System.Linq.Expressions;

static class ExpressionExtensions
{
    // TODO: Better way?
    static readonly Dictionary<Type, int> _typePrecindence = new()
    {
        { typeof(double), 0 },
        { typeof(double?), 1 },
        { typeof(decimal), 2 },
        { typeof(decimal?), 3 }
    };

    public static Expression CallBestOverload(IEnumerable<MethodInfo> overloads, IEnumerable<Expression> arguments)
        => CallBestOverload(instance: null, overloads, arguments);

    public static Expression CallBestOverload(Expression? instance, IEnumerable<MethodInfo> overloads, IEnumerable<Expression> arguments)
    {
        var argumentsList = arguments.ToList();
        var overloadsMap = overloads
            .Where(o => o.GetParameters().Length == argumentsList.Count)
            .ToDictionary(
                o => o.GetParameters().Select(p => p.ParameterType).ToList(),
                o => o,
                new SequenceEqualComparer<Type>());

        // Try for an exact match
        if (overloadsMap.TryGetValue(argumentsList.Select(a => a.Type).ToList(), out var match))
        {
            return Expression.Call(instance, match, argumentsList);
        }

        if (overloadsMap.Count == 0)
        {
            var method = overloadsMap.Values.First();

            throw new PowerFxLinqException($"No overload of '{method.DeclaringType!.Name}.{method.Name}' takes {argumentsList.Count} arguments.");
        }

        // Convert to undrlying types if overloads don't support nullable
        var nullableSupported = argumentsList.Count == 0
            || overloadsMap.Keys.SelectMany(l => l).Any(t => t.IsNullable());
        if (!nullableSupported)
        {
            for (var i = 0; i < argumentsList.Count; i++)
            {
                var argument = argumentsList[i];
                var underlyingType = Nullable.GetUnderlyingType(argument.Type);
                if (underlyingType is not null)
                    argumentsList[i] = Expression.Convert(argument, underlyingType);
            }

            // Try again for an exact match
            if (overloadsMap.TryGetValue(argumentsList.Select(a => a.Type).ToList(), out match))
            {
                return Expression.Call(instance, match, argumentsList);
            }
        }

        // TODO: Look for the best upcast overlad
        // TODO: Resort to the best downcast overload
        {
            var method = overloadsMap.Values.First();
            throw new UnreachableException($"Inconcievable! No overload of '{method.DeclaringType!.Name}.{method.Name}' found.");
        }
    }

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

    public static BinaryExpression LiftAndModulo(Expression left, Expression right)
    {
        (left, right) = Lift(left, right);

        return Expression.Modulo(left, right);
    }

    static (Expression Left, Expression Right) Lift(Expression left, Expression right)
    {
        var leftType = left.Type;
        var rightType = right.Type;

        return leftType == rightType
            ? (left, right)
            : _typePrecindence[leftType] > _typePrecindence[rightType]
                ? (left, Expression.Convert(right, leftType))
                : (Expression.Convert(left, rightType), right);
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
