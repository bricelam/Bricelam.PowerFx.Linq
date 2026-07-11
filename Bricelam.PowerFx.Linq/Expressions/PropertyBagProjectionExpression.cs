using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Expressions;

class PropertyBagProjectionExpression : Expression, IPropertyBagProjection
{
    static readonly Type _dictionaryType = typeof(Dictionary<string, object?>);
    static readonly MethodInfo _addMethod = _dictionaryType
        .GetMethod(nameof(Dictionary<string, object?>.Add), [typeof(string), typeof(object)])!;

    public PropertyBagProjectionExpression(
        Expression source,
        IEnumerable<KeyValuePair<string, Expression>> properties,
        ParameterExpression rangeVariable)
    {
        Source = source;
        RangeVariable = rangeVariable;
        Properties = properties.ToDictionary();
    }

    public override bool CanReduce
        => true;

    public override ExpressionType NodeType
        => ExpressionType.Extension;

    public override Type Type
        => typeof(IQueryable<Dictionary<string, object?>>);

    public Expression Source { get; }
    public IReadOnlyDictionary<string, Expression> Properties { get; }
    public ParameterExpression RangeVariable { get; }

    public override Expression Reduce()
        => Source.Select(
            Lambda(
                ListInit(
                    New(_dictionaryType),
                    Properties.Select(
                        c => ElementInit(
                            _addMethod,
                            Constant(c.Key),
                            Convert(c.Value, typeof(object))))),
                RangeVariable));
}
