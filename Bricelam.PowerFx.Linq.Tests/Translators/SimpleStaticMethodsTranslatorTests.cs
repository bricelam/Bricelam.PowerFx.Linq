namespace Bricelam.PowerFx.Linq.Translators;

public class SimpleStaticMethodsTranslatorTests : TranslatorTestBase
{
    [Fact]
    public void Max()
        => FuncTest("Max(0, 1)", 1.0);

    [Fact]
    public void Max_double()
        => FuncTest("Max(0, Value)", new { Value = 1.0 }, 1.0);

    [Fact(Skip = "Needs better type handling")]
    public void Max_nullable_double()
        => FuncTest("Max(0, Value)", new { Value = default(double?) }, 0.0);

    [Fact]
    public void Min()
        => FuncTest("Min(0, 1)", 0.0);

    [Fact]
    public void Min_double()
        => FuncTest("Min(0, Value)", new { Value = 1.0 }, 0.0);

    [Fact(Skip = "Needs better type handling")]
    public void Min_nullable_double()
        => FuncTest("Min(0, Value)", new { Value = default(double?) }, 0.0);
}
