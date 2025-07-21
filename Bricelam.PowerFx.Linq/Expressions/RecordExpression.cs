using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Expressions;

class RecordExpression : Expression
{
    static readonly Type _dictionaryType = typeof(Dictionary<string, object?>);
    static readonly MethodInfo _dictionaryAddMethod = _dictionaryType
        .GetMethod(nameof(Dictionary<string, object?>.Add), [typeof(string), typeof(object)])!;

    public RecordExpression(IReadOnlyDictionary<string, Expression> fields)
        => Fields = fields;

    public override bool CanReduce
        => true;

    public override ExpressionType NodeType
        => ExpressionType.Extension;

    public override Type Type
        => _dictionaryType;

    public IReadOnlyDictionary<string, Expression> Fields { get; }

    public static RecordExpression FromValue(Expression value)
        => new RecordExpression(
            new Dictionary<string, Expression>
            {
                { "Value", value }
            });

    public override Expression Reduce()
        => ListInit(
            New(_dictionaryType),
            Fields
                .Select(
                    f => ElementInit(
                        _dictionaryAddMethod,
                        Constant(f.Key),
                        Convert(f.Value, typeof(object)))));
}
