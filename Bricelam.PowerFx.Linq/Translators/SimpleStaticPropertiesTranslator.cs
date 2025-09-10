using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleStaticPropertiesTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, PropertyInfo> _map = new()
    {
        { "Now", typeof(DateTime).GetProperty(nameof(DateTime.Now))! },
        { "Today", typeof(DateTime).GetProperty(nameof(DateTime.Today))! },
        { "UTCNow", typeof(DateTime).GetProperty(nameof(DateTime.UtcNow))! }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
    {
        if (_map.TryGetValue(functionName, out var property))
        {
            return Expression.Property(null, property);
        }

        return null;
    }
}
