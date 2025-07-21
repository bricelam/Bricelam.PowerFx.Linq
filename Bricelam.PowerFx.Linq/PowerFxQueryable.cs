using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq;

/// <summary>
/// Provides Power Fx queryable extensions.
/// </summary>
/// <seealso href="https://learn.microsoft.com/power-platform/power-fx/overview">Microsoft Power Fx overview</seealso>
public static class PowerFxQueryable
{
    static readonly Type _dictionaryType = typeof(Dictionary<string, object?>);
    static readonly MethodInfo _addMethod = _dictionaryType
        .GetMethod(nameof(Dictionary<string, object?>.Add), [typeof(string), typeof(object)])!;

    /// <summary>
    /// Adds columns to a sequence of values.
    /// </summary>
    /// <typeparam name="TSource">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="columns">The name-formula pairs of columns to add.</param>
    /// <returns>A queryable whose elements are the result of adding the coluns to each elelemt of <paramref name="source"/>.</returns>
    public static IQueryable<Dictionary<string, object?>> AddColumns<TSource>(
        this IQueryable<TSource> source,
        params (string Name, string Formula)[] columns)
        => AddColumns(source, config: null, columns);

    /// <summary>
    /// Adds columns to a sequence of values.
    /// </summary>
    /// <typeparam name="TSource">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="config">The configuration to use during translation of the formulas.</param>
    /// <param name="columns">The name-formula pairs of columns to add.</param>
    /// <returns>A queryable whose elements are the result of adding the coluns to each elelemt of <paramref name="source"/>.</returns>
    // TODO: An overlad that takes named formlas directly?
    // TODO: A version that returns dynamic (via ExpandoObject)?
    public static IQueryable<Dictionary<string, object?>> AddColumns<TSource>(
        this IQueryable<TSource> source,
        PowerFxLinqConfig? config,
        params (string Name, string Formula)[] columns)
    {
        var e = Expression.Parameter(typeof(TSource), "e");

        var initializers = new List<ElementInit>();

        foreach (var property in typeof(TSource).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            initializers.Add(
                Expression.ElementInit(
                    _addMethod,
                    Expression.Constant(property.Name),
                    Expression.Convert(Expression.Property(e, property), typeof(object))));
        }

        var context = new PowerFxTranslatorContext(config, e);

        foreach (var column in columns)
        {
            initializers.Add(
                Expression.ElementInit(
                    _addMethod,
                    Expression.Constant(column.Name),
                    Expression.Convert(context.Translate(column.Formula), typeof(object))));
        }

        var selector = Expression.Lambda<Func<TSource, Dictionary<string, object?>>>(
            Expression.ListInit(Expression.New(_dictionaryType), initializers),
            e);

        return source.Select(selector);
    }

    // TODO: Consder calling it ForAll
    //public static IQueryable<Dictionary<string, object?>> Select<TSource>(
    //    this IQueryable<TSource> source,
    //    string formula);
}
