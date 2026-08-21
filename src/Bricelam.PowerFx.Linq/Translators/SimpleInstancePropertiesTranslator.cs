using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Translators;

class SimpleInstancePropertiesTranslator : IFunctionCallTranslator
{
    static readonly Dictionary<string, PropertyInfo> _map = new()
    {
        { "Day", typeof(DateTime).GetProperty(nameof(DateTime.Day))! },
        { "Hour", typeof(DateTime).GetProperty(nameof(DateTime.Hour))! },

        // TODO: Handle null
        { "Len", typeof(string).GetProperty(nameof(string.Length))! },

        { "Minute", typeof(DateTime).GetProperty(nameof(DateTime.Minute))! },
        { "Month", typeof(DateTime).GetProperty(nameof(DateTime.Month))! },
        { "Second", typeof(DateTime).GetProperty(nameof(DateTime.Second))! },

        // TODO: Convert to int; handle WeekdayFirst parameter
        { "Weekday", typeof(DateTime).GetProperty(nameof(DateTime.DayOfWeek))! },

        { "Year", typeof(DateTime).GetProperty(nameof(DateTime.Year))! }
    };

    public Expression? Translate(string functionName, IReadOnlyList<Expression> arguments)
    {
        if (_map.TryGetValue(functionName, out var property))
        {
            return Expression.Property(arguments[0], property);
        }

        return null;
    }
}
