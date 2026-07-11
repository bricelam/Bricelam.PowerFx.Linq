using System.Linq.Expressions;
using Bricelam.PowerFx.Linq.Expressions;
using Bricelam.PowerFx.Linq.Reflection;

namespace Bricelam.PowerFx.Linq;

/// <summary>
/// Provides Power Fx queryable extensions.
/// </summary>
/// <seealso href="https://learn.microsoft.com/power-platform/power-fx/overview">Microsoft Power Fx overview</seealso>
public static class PowerFxQueryable
{
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
    public static IQueryable<Dictionary<string, object?>> AddColumns<TSource>(
        this IQueryable<TSource> source,
        PowerFxLinqConfig? config,
        params (string Name, string Formula)[] columns)
    {
        ArgumentNullException.ThrowIfNull(columns);

        Expression sourceExpression;
        var columnExpressions = new Dictionary<string, Expression>();
        ParameterExpression e;
        PropertyProvider propertyProvider;

        if (source.Expression.TryGetPropertyBagProjection(out var projection))
        {
            sourceExpression = projection.Source;
            e = projection.RangeVariable;
            propertyProvider = PropertyProvider.Create(e.Type, sourceExpression);

            foreach (var property in projection.Properties)
            {
                columnExpressions.Add(property.Key, property.Value);
            }
        }
        else
        {
            sourceExpression = source.Expression;
            e = Expression.Parameter(typeof(TSource), "e");
            propertyProvider = PropertyProvider.Create(e.Type, sourceExpression);

            foreach (var property in propertyProvider.GetProperties())
            {
                columnExpressions.Add(property.Name, property.CreateAccessExpression(e));
            }
        }

        var context = new PowerFxTranslatorContext(config, e, propertyProvider);

        foreach (var column in columns)
        {
            columnExpressions.Add(column.Name, context.Translate(column.Formula));
        }

        return source.Provider.CreateQuery<Dictionary<string, object?>>(
            new PropertyBagProjectionExpression(
                sourceExpression,
                columnExpressions,
                e));
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

        Expression sourceExpression;
        var columns = new Dictionary<string, Expression>();
        ParameterExpression e;

        if (source.Expression.TryGetPropertyBagProjection(out var projection))
        {
            sourceExpression = projection.Source;
            e = projection.RangeVariable;

            var shownColumns = new HashSet<string>();
            foreach (var property in projection.Properties)
            {
                if (columnNames.Contains(property.Key))
                {
                    columns.Add(property.Key, property.Value);
                    shownColumns.Add(property.Key);
                }
            }

            if (shownColumns.Count != columnNames.Length)
            {
                throw new PowerFxLinqException(
                    "Columns not found: " + string.Join(", ", columnNames.Except(shownColumns)));
            }
        }
        else
        {
            sourceExpression = source.Expression;
            e = Expression.Parameter(typeof(TSource), "e");
            var propertyProvider = PropertyProvider.Create(e.Type, sourceExpression);

            foreach (var columnName in columnNames)
            {
                var property = propertyProvider.GetProperty(columnName)
                    ?? throw new PowerFxLinqException($"Column '{columnName}' not found.");

                columns.Add(property.Name, property.CreateAccessExpression(e));
            }
        }

        return source.Provider.CreateQuery<Dictionary<string, object?>>(
            new PropertyBagProjectionExpression(
                sourceExpression,
                columns,
                e));
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

        Expression sourceExpression;
        var columns = new Dictionary<string, Expression>();
        ParameterExpression e;

        if (source.Expression.TryGetPropertyBagProjection(out var projection))
        {
            sourceExpression = projection.Source;
            e = projection.RangeVariable;

            var droppedColumns = new HashSet<string>();
            foreach (var property in projection.Properties)
            {
                if (columnNames.Contains(property.Key))
                {
                    droppedColumns.Add(property.Key);
                }
                else
                {
                    columns.Add(property.Key, property.Value);
                }
            }

            if (droppedColumns.Count != columnNames.Length)
            {
                throw new PowerFxLinqException(
                    "Columns not found: " + string.Join(", ", columnNames.Except(droppedColumns)));
            }
        }
        else
        {
            sourceExpression = source.Expression;
            e = Expression.Parameter(typeof(TSource), "e");
            var propertyProvider = PropertyProvider.Create(e.Type, sourceExpression);

            var columnsNotRemoved = new HashSet<string>(columnNames);
            foreach (var property in propertyProvider.GetProperties())
            {
                if (columnsNotRemoved.Remove(property.Name))
                    continue;

                columns.Add(property.Name, property.CreateAccessExpression(e));
            }

            if (columnsNotRemoved.Count != 0)
                throw new PowerFxLinqException("Columns not found: " + string.Join(", ", columnsNotRemoved));
        }

        return source.Provider.CreateQuery<Dictionary<string, object?>>(
            new PropertyBagProjectionExpression(
                sourceExpression,
                columns,
                e));
    }

    // TODO
    //public static IQueryable<Dictionary<string, object?>> ForAll<TSource>(
    //    this IQueryable<TSource> source,
    //    string formula);
}
