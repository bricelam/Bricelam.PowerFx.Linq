using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Expressions;

class PropertyBagProjectionInfo : IPropertyBagProjection
{
    public required Expression Source { get; set; }
    public required IReadOnlyDictionary<string, Expression> Properties { get; set; }
    public required ParameterExpression RangeVariable { get; set; }
}
