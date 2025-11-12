using System.Linq.Expressions;
using System.Reflection;
using Bricelam.PowerFx.Linq.Reflection;

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
    /// <param name="columns">A dictionary of name-formula pairs of columns to add.</param>
    /// <returns>A queryable whose elements are the result of adding the coluns to each elelemt of <paramref name="source"/>.</returns>
    public static IQueryable<Dictionary<string, object?>> AddColumns<TSource>(
        this IQueryable<TSource> source,
        IEnumerable<KeyValuePair<string, string>> columns)
        => AddColumns(source, columns.Select(c => (c.Key, c.Value)).ToArray());

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
    /// <param name="columns">A dictionary of name-formula pairs of columns to add.</param>
    /// <returns>A queryable whose elements are the result of adding the coluns to each elelemt of <paramref name="source"/>.</returns>
    public static IQueryable<Dictionary<string, object?>> AddColumns<TSource>(
        this IQueryable<TSource> source,
        PowerFxLinqConfig? config,
        IEnumerable<KeyValuePair<string, string>> columns)
        => AddColumns(source, config, columns.Select(c => (c.Key, c.Value)).ToArray());

    /// <summary>
    /// Adds columns to a sequence of values.
    /// </summary>
    /// <typeparam name="TSource">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="config">The configuration to use during translation of the formulas.</param>
    /// <param name="columns">The name-formula pairs of columns to add.</param>
    /// <returns>A queryable whose elements are the result of adding the coluns to each elelemt of <paramref name="source"/>.</returns>
    // TODO: A version that returns dynamic (via ExpandoObject)?
    // TODO: Rewrite any existing Select
    public static IQueryable<Dictionary<string, object?>> AddColumns<TSource>(
        this IQueryable<TSource> source,
        PowerFxLinqConfig? config,
        params (string Name, string Formula)[] columns)
    {
        ArgumentNullException.ThrowIfNull(columns);

        var e = Expression.Parameter(typeof(TSource), "e");

        var propertyProvider = PropertyProvider.Create(typeof(TSource), source.Expression);
        var initializers = new List<ElementInit>();

        foreach (var property in propertyProvider.GetProperties())
        {
            initializers.Add(
                Expression.ElementInit(
                    _addMethod,
                    Expression.Constant(property.Name),
                    Expression.Convert(property.CreateAccessExpression(e), typeof(object))));
        }

        var context = new PowerFxTranslatorContext(config, e, propertyProvider);

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

    /// <summary>
    /// Removes all but the specified columns from a sequence of values.
    /// </summary>
    /// <typeparam name="TSource">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="columnNames">The names of columns to include in the result.</param>
    /// <returns>A queryable whose elements are the result of removing all but the columns specified in <paramref name="columnNames"/> from each element of <paramref name="source"/>.</returns>
    public static IQueryable<Dictionary<string, object?>> ShowColumns<TSource>(
        this IQueryable<TSource> source,
        params string[] columnNames)
    {
        ArgumentNullException.ThrowIfNull(columnNames);

        // TODO: Handle more initializers
        if (typeof(TSource) == typeof(Dictionary<string, object?>)
            && source.Expression.IsSelect(out var newSource, out var oldSelector))
        {
            var oldListInit = (ListInitExpression)oldSelector.Body;

            var newInitializers = new List<ElementInit>();
            var shownColumns = new HashSet<string>();
            foreach (var oldInitializer in oldListInit.Initializers)
            {
                var columnName = (string)((ConstantExpression)oldInitializer.Arguments[0]).Value!;
                if (columnNames.Contains(columnName))
                {
                    newInitializers.Add(oldInitializer);
                    shownColumns.Add(columnName);
                }
            }

            if (shownColumns.Count != columnNames.Length)
            {
                throw new PowerFxLinqException(
                    "Columns not found: " + string.Join(", ", columnNames.Except(shownColumns)));
            }

            var newSelector = Expression.Lambda(
                Expression.ListInit(oldListInit.NewExpression, newInitializers),
                oldSelector.Parameters);

            return source.Provider.CreateQuery<Dictionary<string, object?>>(
                newSource.Select(newSelector));
        }

        var e = Expression.Parameter(typeof(TSource), "e");

        var propertyProvider = PropertyProvider.Create(typeof(TSource), source.Expression);
        var initializers = new List<ElementInit>();

        foreach (var columnName in columnNames)
        {
            var property = propertyProvider.GetProperty(columnName)
                ?? throw new PowerFxLinqException($"Column '{columnName}' not found.");

            initializers.Add(
                Expression.ElementInit(
                    _addMethod,
                    Expression.Constant(property.Name),
                    Expression.Convert(property.CreateAccessExpression(e), typeof(object))));
        }

        var selector = Expression.Lambda<Func<TSource, Dictionary<string, object?>>>(
            Expression.ListInit(Expression.New(_dictionaryType), initializers),
            e);

        return source.Select(selector);
    }

    /// <summary>
    /// Removes the specified columns from a sequence of values.
    /// </summary>
    /// <typeparam name="TSource">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="columnNames">The names of columns to exclude from the result.</param>
    /// <returns>A queryable whose elements are the result of removing the columns specified in <paramref name="columnNames"/> from each element of <paramref name="source"/>.</returns>
    public static IQueryable<Dictionary<string, object?>> DropColumns<TSource>(
        this IQueryable<TSource> source,
        params string[] columnNames)
    {
        ArgumentNullException.ThrowIfNull(columnNames);

        // TODO: Handle more initializers
        if (typeof(TSource) == typeof(Dictionary<string, object?>)
            && source.Expression.IsSelect(out var newSource, out var oldSelector))
        {
            var oldListInit = (ListInitExpression)oldSelector.Body;

            var newInitializers = new List<ElementInit>();
            var droppedColumns = new HashSet<string>();
            foreach (var oldInitializer in oldListInit.Initializers)
            {
                var columnName = (string)((ConstantExpression)oldInitializer.Arguments[0]).Value!;
                if (!columnNames.Contains(columnName))
                {
                    newInitializers.Add(oldInitializer);
                }
                else
                {
                    droppedColumns.Add(columnName);
                }
            }

            if (droppedColumns.Count != columnNames.Length)
            {
                throw new PowerFxLinqException(
                    "Columns not found: " + string.Join(", ", columnNames.Except(droppedColumns)));
            }

            var newSelector = Expression.Lambda(
                Expression.ListInit(
                    oldListInit.NewExpression,
                    oldListInit.Initializers
                        .Where(i => !columnNames.Contains((string)((ConstantExpression)i.Arguments[0]).Value!))),
                oldSelector.Parameters);

            return source.Provider.CreateQuery<Dictionary<string, object?>>(
                newSource.Select(newSelector));
        }

        var e = Expression.Parameter(typeof(TSource), "e");

        var propertyProvider = PropertyProvider.Create(typeof(TSource), source.Expression);
        var columnsNotRemoved = new HashSet<string>(columnNames);
        var initializers = new List<ElementInit>();

        foreach (var property in propertyProvider.GetProperties())
        {
            if (columnsNotRemoved.Remove(property.Name))
                continue;

            initializers.Add(
                Expression.ElementInit(
                    _addMethod,
                    Expression.Constant(property.Name),
                    Expression.Convert(property.CreateAccessExpression(e), typeof(object))));
        }

        if (columnsNotRemoved.Count != 0)
            throw new PowerFxLinqException("Columns not found: " + string.Join(", ", columnsNotRemoved));

        var selector = Expression.Lambda<Func<TSource, Dictionary<string, object?>>>(
            Expression.ListInit(Expression.New(_dictionaryType), initializers),
            e);

        return source.Select(selector);
    }

    // TODO
    //public static IQueryable<Dictionary<string, object?>> ForAll<TSource>(
    //    this IQueryable<TSource> source,
    //    string formula);
}
