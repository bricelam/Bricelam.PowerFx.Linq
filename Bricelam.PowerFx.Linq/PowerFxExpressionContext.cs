using System.Linq.Expressions;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;

namespace Bricelam.PowerFx.Linq;

class PowerFxExpressionContext
{
    public ParameterExpression? ThisRecord { get; set; }

    public Expression Translate(string formula)
    {
        var config = new PowerFxConfig();
        config.EnableUTCFunctions();

        if (ThisRecord is not null)
        {
            config.SymbolTable.AddVariable("ThisRecord", FormulaType.UntypedObject);
            foreach (var property in ThisRecord.Type.GetProperties())
            {
                config.SymbolTable.AddVariable(
                    property.Name,
                    // TODO: Lift all numbers to decimal?
                    PrimitiveValueConversions.TryGetFormulaType(property.PropertyType, out var formulaType)
                        ? formulaType
                        : FormulaType.UntypedObject);
            }
        }

        var engine = new Engine(config);
        var checkResult = engine.Check(formula);
        checkResult.ThrowOnErrors();

        return checkResult.Parse.Root.Accept(new LinqTranslatingVisitor(), this);
    }
}
