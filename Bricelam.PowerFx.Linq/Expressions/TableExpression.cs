using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Expressions;

// TODO: Consider optimizations for single-column tables
class TableExpression : Expression
{
    static readonly Type _listType = typeof(List<Dictionary<string, object?>>);
    static readonly MethodInfo _listAddMethod = _listType
        .GetMethod(nameof(List<Dictionary<string, object?>>.Add), [typeof(Dictionary<string, object?>)])!;

    public TableExpression(IReadOnlyCollection<RecordExpression> records)
        => Records = records;

    public override bool CanReduce
        => true;

    public override ExpressionType NodeType
        => ExpressionType.Extension;

    public override Type Type
        => _listType;

    public IReadOnlyCollection<RecordExpression> Records { get; }

    public override Expression Reduce()
        => ListInit(
            New(_listType),
            _listAddMethod,
            Records);
}
