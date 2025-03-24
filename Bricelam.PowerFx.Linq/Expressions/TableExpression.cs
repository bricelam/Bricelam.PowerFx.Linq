using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Expressions;

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

    //public bool IsSingleColumnTable()
    //{
    //    if (Records.Count == 0)
    //        return false;

    //    var firstRecord = Records.First();
    //    if (firstRecord.Fields.Count != 1)
    //        return false;

    //    return firstRecord.Fields.Keys.First() == "Value";
    //}
}
