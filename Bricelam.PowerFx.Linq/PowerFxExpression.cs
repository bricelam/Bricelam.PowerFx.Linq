using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq;

/// <summary>
/// Provides methods for translating Power Fx formulas into lambda expressions.
/// </summary>
/// <seealso href="https://learn.microsoft.com/power-platform/power-fx/overview">Microsoft Power Fx overview</seealso>
public static class PowerFxExpression
{
    /// <summary>
    /// Translates a Power Fx formula into an <see cref="System.Action"/> expression.
    /// </summary>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Action> Action(string formula)
        => Action(config: null, formula);

    /// <summary>
    /// Translates a Power Fx formula into an <see cref="System.Action"/> expression.
    /// </summary>
    /// <param name="config">The configuration to use during translation.</param>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Action> Action(PowerFxLinqConfig? config, string formula)
    {
        var context = new PowerFxTranslatorContext(config, thisRecord: null);

        return Expression.Lambda<Action>(context.Translate(formula));
    }

    /// <summary>
    /// Translates a Power Fx formula into an <see cref="System.Action{T}"/> expression.
    /// </summary>
    /// <typeparam name="T">The parameter (ThisRecord) type.</typeparam>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Action<T>> Action<T>(string formula)
        => Action<T>(config: null, formula);

    /// <summary>
    /// Translates a Power Fx formula into an <see cref="System.Action{T}"/> expression.
    /// </summary>
    /// <typeparam name="T">The parameter (ThisRecord) type.</typeparam>
    /// <param name="config">The configuration to use during translation.</param>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Action<T>> Action<T>(PowerFxLinqConfig? config, string formula)
    {
        var thisRecord = Expression.Parameter(typeof(T), "x");
        var context = new PowerFxTranslatorContext(config, thisRecord);

        return Expression.Lambda<Action<T>>(context.Translate(formula), thisRecord);
    }

    /// <summary>
    /// Translates formulas into <see cref="System.Func{TResult}"/> expressions.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<TResult>> Func<TResult>(string formula)
        => Func<TResult>(config: null, formula);

    /// <summary>
    /// Translates formulas into <see cref="System.Func{TResult}"/> expressions.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="config">The configuration to use during translation.</param>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<TResult>> Func<TResult>(PowerFxLinqConfig? config, string formula)
    {
        var context = new PowerFxTranslatorContext(config, thisRecord: null);

        return Expression.Lambda<Func<TResult>>(context.Translate(formula));
    }

    /// <summary>
    /// Translates record-scoped formulas into <see cref="System.Func{T,TResult}"/> expressions.
    /// </summary>
    /// <typeparam name="T">The parameter (ThisRecord) type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<T, TResult>> Func<T, TResult>(string formula)
        => Func<T, TResult>(config: null, formula);

    /// <summary>
    /// Translates record-scoped formulas into <see cref="System.Func{T,TResult}"/> expressions.
    /// </summary>
    /// <typeparam name="T">The parameter (ThisRecord) type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="config">The configuration to use during translation.</param>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<T, TResult>> Func<T, TResult>(PowerFxLinqConfig? config, string formula)
    {
        var thisRecord = Expression.Parameter(typeof(T), "x");
        var context = new PowerFxTranslatorContext(config, thisRecord);

        return Expression.Lambda<Func<T, TResult>>(context.Translate(formula), thisRecord);
    }

    /// <summary>
    /// Translates record-scoped formulas into predicate expressions.
    /// </summary>
    /// <typeparam name="T">The parameter (ThisRecord) type.</typeparam>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<T, bool>> Predicate<T>(string formula)
        => Predicate<T>(config: null, formula);

    /// <summary>
    /// Translates record-scoped formulas into predicate expressions.
    /// </summary>
    /// <typeparam name="T">The parameter (ThisRecord) type.</typeparam>
    /// <param name="config">The configuration to use during translation.</param>
    /// <param name="formula">The formula.</param>
    /// <returns>The expression.</returns>
    public static Expression<Func<T, bool>> Predicate<T>(PowerFxLinqConfig? config, string formula)
        => Func<T, bool>(config, formula);
}
