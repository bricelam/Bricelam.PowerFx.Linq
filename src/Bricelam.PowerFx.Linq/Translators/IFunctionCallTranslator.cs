using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Translators;

interface IFunctionCallTranslator
{
    Expression? Translate(string functionName, IReadOnlyList<Expression> arguments);
}
