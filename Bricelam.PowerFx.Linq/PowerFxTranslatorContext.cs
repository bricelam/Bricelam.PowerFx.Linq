using System.Linq.Expressions;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;

namespace Bricelam.PowerFx.Linq;

class PowerFxTranslatorContext
{
    readonly ParameterExpression? _thisRecord;
    readonly IReadOnlyDictionary<string, string>? _namedFormulas;
    readonly Engine _engine;
    readonly ParserOptions _parserOptions;

    public PowerFxTranslatorContext(PowerFxLinqConfig? linqConfig, ParameterExpression? thisRecord)
    {
        _thisRecord = thisRecord;
        _namedFormulas = linqConfig is null ? null : new Dictionary<string, string>(linqConfig.NamedFormulas);

        var config = new PowerFxConfig();

        if (_thisRecord is not null)
        {
            config.SymbolTable.AddVariable("ThisRecord", FormulaType.UntypedObject);
            foreach (var property in _thisRecord.Type.GetProperties())
            {
                config.SymbolTable.AddVariable(
                    property.Name,
                    PrimitiveValueConversions.TryGetFormulaType(property.PropertyType, out var formulaType)
                        ? formulaType
                        : FormulaType.UntypedObject);
            }
        }

        if (_namedFormulas is not null)
        {
            foreach (var name in _namedFormulas.Keys)
            {
                config.SymbolTable.AddVariable(name, FormulaType.Unknown);
            }
        }

        linqConfig?.ConfigureEngine?.Invoke(config);

        _engine = new Engine(config);

        _parserOptions = _engine.GetDefaultParserOptionsCopy();
        _parserOptions.NumberIsFloat = true;
        linqConfig?.ConfigureParser?.Invoke(_parserOptions);
    }

    public bool NumberIsDecimal
        => !_parserOptions.NumberIsFloat;

    public Expression Translate(string formula)
    {
        var checkResult = _engine.Check(formula, _parserOptions);
        checkResult.ThrowOnErrors();

        return checkResult.Parse.Root.Accept(new PowerFxTranslator(), this);
    }

    public Expression? Bind(string identifier)
    {
        if (_thisRecord is not null)
        {
            if (identifier == "ThisRecord")
            {
                return _thisRecord;
            }

            var property = _thisRecord.Type.GetProperty(identifier);
            if (property is not null)
            {
                return Expression.Property(_thisRecord, property);
            }
        }

        if (_namedFormulas is not null
            && _namedFormulas.TryGetValue(identifier, out var namedFormula))
        {
            return Translate(namedFormula);
        }

        return null;
    }
}
