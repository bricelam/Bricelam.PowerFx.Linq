using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Expressions;

interface IPropertyBagProjection
{
    Expression Source { get; }
    IReadOnlyDictionary<string, Expression> Properties { get; }
    ParameterExpression RangeVariable { get; }
}
