using System.Linq.Expressions;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;

namespace Bricelam.PowerFx.Linq;

// TODO: Named formulas
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
                // TODO: Handle Dictionary<string, object>
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

    public Expression? Bind(string identifier)
    {
        if (ThisRecord is not null)
        {
            if (identifier == "ThisRecord")
            {
                return ThisRecord;
            }

            // TODO: Handle Dictionary<string, object>
            var property = ThisRecord.Type.GetProperty(identifier);
            if (property is not null)
            {
                // TODO: Lift all numbers to decimal?
                return Expression.Property(ThisRecord, property);
            }
        }

        return null;
    }
}
