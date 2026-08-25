namespace Bricelam.PowerFx.Linq.Translators;

public class SimpleStaticMethodsTranslatorTests : TranslatorTestBase
{
    [Fact]
    public void Max()
        => FuncTest("Max(0, 1)", 1.0);

    [Fact]
    public void Max_byte()
        => FuncTest("Max(0, Value)", new { Value = (byte)1 }, 1.0);

    [Fact]
    public void Max_short()
        => FuncTest("Max(0, Value)", new { Value = (short)1 }, 1.0);

    [Fact]
    public void Max_int()
        => FuncTest("Max(0, Value)", new { Value = 1 }, 1.0);

    [Fact]
    public void Max_long()
        => FuncTest("Max(0, Value)", new { Value = 1L }, 1.0m);

    [Fact]
    public void Max_float()
        => FuncTest("Max(0, Value)", new { Value = 1.0f }, 1.0);

    [Fact]
    public void Max_double()
        => FuncTest("Max(0, Value)", new { Value = 1.0 }, 1.0);

    [Fact(Skip = "TODO: Is this valid?")]
    public void Max_nullable_double()
        => FuncTest("Max(0, Value)", new { Value = default(double?) }, 0.0);

    [Fact]
    public void Max_decimal()
        => FuncTest(DecimalConfig, "Max(0, Value)", new { Value = 1.0m }, 1.0m);

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
    public void Power()
        => FuncTest("Power(2, 3)", 8.0);

    [Fact]
    public void Power_double_nullable()
        => FuncTest("Power(Value, 3)", new { Value = (double?)2.0 }, 8.0);

    [Fact]
    public void Power_decimal()
        => FuncTest(DecimalConfig, "Power(2, 3)", 8.0);

    [Fact]
    public void Power_decimal_nullable()
        => FuncTest(DecimalConfig, "Power(Value, 3)", new { Value = (decimal?)2.0 }, 8.0);

    [Fact]
    public void Power_float()
        => FuncTest("Power(X, Y)", new { X = 2.0f, Y = 3.0f }, 8.0f);

    [Fact]
    public void Power_float_nullable()
        => FuncTest("Power(X, Y)", new { X = (float?)2.0f, Y = (float?)3.0f }, 8.0f);

    [Fact]
    public void Round_double()
        => FuncTest("Round(3.14, 1)", 3.1);

    [Fact]
    public void Round_double_nullable()
        => FuncTest("Round(Value, 1)", new { Value = (double?)3.14 }, 3.1);

    [Fact]
    public void Round_decimal()
        => FuncTest(DecimalConfig, "Round(3.14, 1)", 3.1m);

    [Fact]
    public void Round_decimal_nullable()
        => FuncTest("Round(Value, 1)", new { Value = (decimal?)3.14m }, 3.1m);

    [Fact]
    public void Round_float()
        => FuncTest("Round(Value, 1)", new { Value = 3.14f }, 3.1f);

    [Fact]
    public void Round_float_nullable()
        => FuncTest("Round(Value, 1)", new { Value = (float?)3.14f }, 3.1f);
}
