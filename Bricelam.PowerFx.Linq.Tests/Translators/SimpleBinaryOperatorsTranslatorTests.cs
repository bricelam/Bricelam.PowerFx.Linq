namespace Bricelam.PowerFx.Linq.Translators;

public class SimpleBinaryOperatorsTranslatorTests : TranslatorTestBase
{
    [Fact(Skip = "Needs better type handling")]
    public void Coalesce()
        => FuncTest("Coalesce(Blank(), 0)", 0m);

    [Fact(Skip = "Needs better type handling")]
    public void Coalesce_decimal()
        => FuncTest("Coalesce(Value, 0)", new { Value = 1m }, 1m);

    [Fact]
    public void Coalesce_nullable_decimal()
        => FuncTest("Coalesce(Value, 0)", new { Value = (decimal?)1m }, 1m);

    [Fact]
    public void Coalesce_nullable_decimal_null()
        => FuncTest("Coalesce(Value, 0)", new { Value = default(decimal?) }, 0m);

    [Fact(Skip = "Needs better type handling")]
    public void Coalesce_double()
        => FuncTest("Coalesce(Value, 0)", new { Value = 1.0 }, 1.0m);

    [Fact(Skip = "Needs better type handling")]
    public void Coalesce_nullable_double()
        => FuncTest("Coalesce(Value, 0)", new { Value = (double?)1.0 }, 1.0m);

    [Fact(Skip = "Needs better type handling")]
    public void Coalesce_nullable_double_null()
        => FuncTest("Coalesce(Value, 0)", new { Value = default(double?) }, 0m);

    [Fact]
    public void Sum()
        => FuncTest("Sum(1, 1)", 2m);

    [Fact]
    public void Sum_one()
        => FuncTest("Sum(1)", 1m);

    [Fact]
    public void Sum_many()
        => FuncTest("Sum(1, 1, 1)", 3m);

    [Fact]
    public void Sum_double()
        => FuncTest("Sum(1, Value)", new { Value = 1.0 }, 2m);

    [Fact]
    public void Sum_nullable_double()
        => FuncTest("Sum(1, Value)", new { Value = (double?)1.0 }, 2m);

    [Fact]
    // TODO: Verify this should throw
    public void Sum_nullable_double_null()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("Sum(1, Value)", new { Value = default(double?) }));
}
