using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq;

interface IPowerFxTranslatorContext
{
    bool NumberIsDecimal { get; }
    Expression? Bind(string identifier);
}
