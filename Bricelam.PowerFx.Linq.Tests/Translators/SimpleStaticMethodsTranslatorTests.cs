namespace Bricelam.PowerFx.Linq.Translators;

public class SimpleStaticMethodsTranslatorTests : TranslatorTestBase
{
    [Fact]
    public void Max()
        => FuncTest("Max(0, 1)", 1.0);

    [Fact]
    public void Max_double()
        => FuncTest("Max(0, Value)", new { Value = 1.0 }, 1.0);

    [Fact(Skip = "TODO: Is this valid?")]
    public void Max_nullable_double()
        => FuncTest("Max(0, Value)", new { Value = default(double?) }, 0.0);

    [Fact]
    public void Min()
        => FuncTest("Min(0, 1)", 0.0);

    [Fact]
    public void Min_double()
        => FuncTest("Min(0, Value)", new { Value = 1.0 }, 0.0);

    [Fact(Skip = "TODO: Is this valid?")]
    public void Min_nullable_double()
        => FuncTest("Min(0, Value)", new { Value = default(double?) }, 0.0);

    [Fact]
    public void Round_double()
        => FuncTest("Round(3.14, 1)", 3.1);

    [Fact]
    public void Round_double_nullable()
        => FuncTest("Round(Value, 1)", new { Value = (double?)3.14 }, 3.1);

    [Fact]
    public void Round_decimal()
        => FuncTest(DecimalConfig, "Round(3.14, 1)", 3.1);

    [Fact]
    public void Round_decimal_nullable()
        => FuncTest("Round(Value, 1)", new { Value = (decimal?)3.14m }, 3.1);

    [Fact]
    public void Round_float()
        => FuncTest("Round(Value, 1)", new { Value = 3.14f }, 3.1);

    [Fact]
    public void Round_float_nullable()
        => FuncTest("Round(Value, 1)", new { Value = (float?)3.14f }, 3.1);
}
