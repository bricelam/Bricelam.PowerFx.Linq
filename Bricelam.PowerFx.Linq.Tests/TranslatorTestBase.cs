namespace Bricelam.PowerFx.Linq;

public abstract class TranslatorTestBase
{
    protected static void ActionTest(string formula)
        => ActionTest(config: null, formula);

    protected static void ActionTest(PowerFxLinqConfig? config, string formula)
    {
        var expression = PowerFxExpression.Action(formula);

        expression.Compile().Invoke();
    }

    protected static void ActionTest<T>(string formula, T arg)
        => ActionTest(config: null, formula, arg);

    protected static void ActionTest<T>(PowerFxLinqConfig? config, string formula, T arg)
    {
        var expression = PowerFxExpression.Action<T>(config, formula);

        expression.Compile().Invoke(arg);
    }

    protected static void FuncTest<TResult>(string formula, TResult expected)
        => FuncTest(config: null, formula, expected);

    protected static void FuncTest<TResult>(PowerFxLinqConfig? config, string formula, TResult expected)
    {
        var expression = PowerFxExpression.Func<TResult>(config, formula);

        var actual = expression.Compile().Invoke();

        Assert.Equal(expected, actual);
    }

    protected static void FuncTest<T, TResult>(string formula, T arg, TResult expected)
        => FuncTest(config: null, formula, arg, expected);

    protected static void FuncTest<T, TResult>(PowerFxLinqConfig? config, string formula, T arg, TResult expected)
    {
        var expression = PowerFxExpression.Func<T, TResult>(config, formula);

        var actual = expression.Compile().Invoke(arg);

        Assert.Equal(expected, actual);
    }
}
