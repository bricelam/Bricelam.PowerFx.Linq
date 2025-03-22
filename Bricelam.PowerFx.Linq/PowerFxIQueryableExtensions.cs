using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq;

public static class PowerFxIQueryableExtensions
{
    // DRY (LinqTranslatingVisitor)
    static readonly Type _dictionaryType = typeof(Dictionary<string, object?>);
    static readonly MethodInfo _addMethod = _dictionaryType
        .GetMethod(nameof(Dictionary<string, object?>.Add), [typeof(string), typeof(object)])!;

    // TODO: Binding overlad
    // TODO: Dynamic via ExpandoObject?
    public static IQueryable<Dictionary<string, object?>> AddColumns<TSource>(
        this IQueryable<TSource> source,
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

        var context = new PowerFxExpressionContext
        {
            ThisRecord = e
        };

        foreach (var column in columns)
        {
            initializers.Add(
                Expression.ElementInit(
                    _addMethod,
                    Expression.Constant(column.Name),
                    // TODO: Flow dependencies
                    Expression.Convert(context.Translate(column.Formula), typeof(object))));
        }

        var selector = Expression.Lambda<Func<TSource, Dictionary<string, object?>>>(
            Expression.ListInit(Expression.New(_dictionaryType), initializers),
            e);

        return source.Select(selector);
    }

    // TODO
    //public static IQueryable<Dictionary<string, object?>> Select<TSource>(
    //    this IQueryable<TSource> source,
    //    string formula);
}
