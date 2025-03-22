using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq;

/// <summary>
/// Provides methods for translating formulas into simple lambda expressions.
/// </summary>
public static class PowerFxExpression
{
    /// <summary>
    /// Translates formulas into <see cref="System.Func{TResult}"/> expressions.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<TResult>> Func<TResult>(string formula)
    {
        var context = new PowerFxExpressionContext();

        return Expression.Lambda<Func<TResult>>(
            context.Translate(formula));
    }

    /// <summary>
    /// Translates formulas into <see cref="System.Func{T,TResult}"/> expressions.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<T, TResult>> Func<T, TResult>(string formula)
    {
        var context = new PowerFxExpressionContext
        {
            ThisRecord = Expression.Parameter(typeof(T), "x")
        };

        return Expression.Lambda<Func<T, TResult>>(
            context.Translate(formula),
            context.ThisRecord);
    }

    /// <summary>
    /// Translates formulas into predicate expressions.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<T, bool>> Predicate<T>(string formula)
        => Func<T, bool>(formula);
}
