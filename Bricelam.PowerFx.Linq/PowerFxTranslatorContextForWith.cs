using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq;

class PowerFxTranslatorContextForWith : IPowerFxTranslatorContext
{
    readonly IPowerFxTranslatorContext _outerContext;
    readonly Dictionary<string, Expression> _namedExpressions;

    public PowerFxTranslatorContextForWith(
        IPowerFxTranslatorContext outerContext,
        IReadOnlyDictionary<string, Expression> namedExpressions)
    {
        _outerContext = outerContext;
        _namedExpressions = new Dictionary<string, Expression>(namedExpressions);
    }

    public bool NumberIsDecimal
        => _outerContext.NumberIsDecimal;

    public Expression? Bind(string identifier)
    {
        if (_namedExpressions.TryGetValue(identifier, out var expression))
        {
            return expression;
        }

        return _outerContext.Bind(identifier);
    }
}
