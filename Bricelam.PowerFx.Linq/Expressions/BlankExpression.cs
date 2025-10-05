using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Expressions;

// TODO: Just work directly with ConstantExpression instead?
class BlankExpression : Expression
{
    public override bool CanReduce
        => true;

    public override ExpressionType NodeType
        => ExpressionType.Extension;

    override public Type Type
        => typeof(object);

    public override Expression Reduce()
        => Constant(null);
}
