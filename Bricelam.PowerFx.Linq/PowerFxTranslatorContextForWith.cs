using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq;

class PowerFxTranslatorContextForWith : IPowerFxTranslatorContext
{
    readonly Dictionary<string, Expression> _namedExpressions;

    public PowerFxTranslatorContextForWith(
        bool numberIsDecimal,
        IReadOnlyDictionary<string, Expression> namedExpressions)
    {
        NumberIsDecimal = numberIsDecimal;
        _namedExpressions = new Dictionary<string, Expression>(namedExpressions);
    }

    public bool NumberIsDecimal { get; }

    public Expression? Bind(string identifier)
    {
        if (_namedExpressions.TryGetValue(identifier, out var expression))
        {
            return expression;
        }

        return null;
    }
}
