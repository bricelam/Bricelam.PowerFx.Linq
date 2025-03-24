namespace Bricelam.PowerFx.Linq;

public abstract class TranslatingTestBase
{
    protected static void ActionTest(string formula)
    {
        var expression = PowerFxExpression.Action(formula);

        expression.Compile().Invoke();
    }

    protected static void ActionTest<T>(string formula, T arg)
    {
        var expression = PowerFxExpression.Action<T>(formula);

        expression.Compile().Invoke(arg);
    }

    protected static void FuncTest<TResult>(string formula, TResult expected)
    {
        var expression = PowerFxExpression.Func<TResult>(formula);

        var actual = expression.Compile().Invoke();

        Assert.Equal(expected, actual);
    }

    protected static void FuncTest<T, TResult>(string formula, T arg, TResult expected)
    {
        var expression = PowerFxExpression.Func<T, TResult>(formula);

        var actual = expression.Compile().Invoke(arg);

        Assert.Equal(expected, actual);
    }
}
