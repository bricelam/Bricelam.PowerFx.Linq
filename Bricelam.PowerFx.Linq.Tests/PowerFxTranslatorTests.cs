namespace Bricelam.PowerFx.Linq;

// TODO: Test all types
public class PowerFxTranslatorTests : TranslatorTestBase
{
    [Fact]
    public void Error()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest(">= 25"));

    [Fact]
    public void Blank_formulas_throw()
        => Assert.Throws<PowerFxLinqException>(
            () => ActionTest(""));

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Constant_bool(string formula, bool expected)
        => FuncTest(formula, expected);

    [Theory]
    [InlineData("\"\"", "")]
    [InlineData("\"A\"", "A")]
    [InlineData("\"A1\"", "A1")]
    public void Constant_string(string formula, string expected)
        => FuncTest(formula, expected);

    [Theory]
    [InlineData("1", 1.0)]
    [InlineData("0.1", 0.1)]
    public void Constant_decimal(string formula, decimal expected)
        => FuncTest(formula, expected);

    [Fact]
    public void String_interpolation()
        => FuncTest("$\"A{1}\"", "A1");

    [Fact]
    public void Unary_not()
        => FuncTest("!true", false);

    [Fact]
    public void Unary_minus()
        => FuncTest("-(1)", -1m);

    [Fact]
    public void Unary_minus_double()
        => FuncTest("-Value", new { Value = 1.0 }, -1.0);

    [Fact]
    public void Unary_minus_nullable_double()
        => FuncTest("-Value", new { Value = (double?)1.0 }, (double?)-1.0);

    [Fact]
    public void Unary_minus_nullable_double_null()
        => FuncTest("-Value", new { Value = default(double?) }, default(double?));

    [Fact]
    public void Unary_percent()
        => FuncTest("1%", 0.01m);

    [Fact]
    public void Binary_or()
        => FuncTest("false || true", true);

    [Fact]
    public void Binary_and()
        => FuncTest("true && false", false);

    [Fact]
    public void Binary_concat()
        => FuncTest("\"A\" & \"1\"", "A1");

    [Fact]
    public void Binary_add()
        => FuncTest("1 + 1", 2m);

    [Fact]
    public void Binary_add_double()
        => FuncTest("1 + Value", new { Value = 1.0 }, 2m);

    [Fact]
    public void Binary_add_nullable_double()
        => FuncTest("1 + Value", new { Value = (double?)1.0 }, 2m);

    [Fact]
    public void Binary_add_nullable_double_null()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("1 + Value", new { Value = default(double?) }));

    [Fact]
    public void Binary_sub()
        => FuncTest("1 - 1", 0m);

    [Fact]
    public void Binary_sub_double()
        => FuncTest("1 - Value", new { Value = 1.0 }, 0m);

    [Fact]
    public void Binary_sub_nullable_double()
        => FuncTest("1 - Value", new { Value = (double?)1.0 }, 0m);

    [Fact]
    public void Binary_sub_nullable_double_null()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("1 - Value", new { Value = default(double?) }));

    [Fact]
    public void Binary_mul()
        => FuncTest("1 * 0", 0m);

    [Fact]
    public void Binary_mul_double()
        => FuncTest("1 * Value", new { Value = 0.0 }, 0m);

    [Fact]
    public void Binary_mul_nullable_double()
        => FuncTest("1 * Value", new { Value = (double?)0.0 }, 0m);

    [Fact]
    public void Binary_mul_nullable_double_null()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("1 * Value", new { Value = default(double?) }));

    [Fact]
    public void Binary_div()
        => FuncTest("2 / 2", 1m);

    [Fact]
    public void Binary_div_double()
        => FuncTest("2 / Value", new { Value = 2.0 }, 1m);

    [Fact]
    public void Binary_div_nullable_double()
        => FuncTest("2 / Value", new { Value = (double?)2.0 }, 1m);

    [Fact]
    public void Binary_div_nullable_double_null()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("2 / Value", new { Value = default(double?) }));

    [Fact]
    public void Binary_power()
        => FuncTest("2 ^ 3", 8m);

    [Fact]
    public void Binary_power_double()
        => FuncTest("Value ^ 3", new { Value = 2.0 }, 8.0);

    [Fact]
    public void Binary_power_nullable_double()
        => FuncTest("Value ^ 3", new { Value = (double?)2.0 }, (double?)8.0);

    [Fact]
    public void Binary_power_nullable_double_null()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("Value ^ 3", new { Value = default(double?) }));

    [Fact]
    public void Binary_equal()
        => FuncTest("1 = 1", true);

    [Fact]
    public void Binary_not_equal()
        => FuncTest("1 <> 1", false);

    [Fact]
    public void Binary_less()
        => FuncTest("1 < 2", true);

    [Fact]
    public void Binary_less_equal()
        => FuncTest("1 <= 2", true);

    [Fact]
    public void Binary_greater()
        => FuncTest("2 > 1", true);

    [Fact]
    public void Binary_greater_equal()
        => FuncTest("2 >= 1", true);

    [Fact]
    public void Binary_in_string()
        => FuncTest("\"A\" in \"ABC\"", true);

    [Fact]
    public void Binary_exactin_string()
        => FuncTest("\"A\" exactin \"ABC\"", true);

    [Fact]
    public void Record()
        => FuncTest("{Value:1}", new Dictionary<string, object?> { { "Value", 1m } });

    [Fact]
    public void Table_empty()
        => FuncTest("[]", new List<Dictionary<string, object?>>(0));

    [Fact]
    public void Table_value()
        => FuncTest("[1]", new List<Dictionary<string, object?>> { new() { { "Value", 1m } } });

    [Fact]
    public void Table_value_expression()
        => FuncTest("[1 + 1]", new List<Dictionary<string, object?>> { new() { { "Value", 2m } } });

    [Fact]
    public void Table_record()
    => FuncTest("[{Value:1}]", new List<Dictionary<string, object?>> { new() { { "Value", 1m } } });

    [Fact]
    public void Named_formulas()
    {
        var config = new PowerFxLinqConfig();
        config.NamedFormulas.Add("R", "D / 2");

        FuncTest(config, "2 * Pi() * R", new { D = 2m }, 2m * (decimal)Math.PI);
    }

    [Fact]
    public void Call_Average()
        => FuncTest("Average(1, 1)", 1m);

    [Fact]
    public void Call_Average_one()
        => FuncTest("Average(1)", 1m);

    [Fact]
    public void Call_Average_many()
        => FuncTest("Average(1, 1, 1)", 1m);

    [Fact]
    public void Call_Average_double()
        => FuncTest("Average(Value)", new { Value = 1.0 }, 1.0);

    [Fact]
    public void Call_Average_nullable_double()
        => FuncTest("Average(Value)", new { Value = (double?)1.0 }, (double?)1.0);

    [Fact]
    // TODO: Verify this should throw
    public void Call_Average_nullable_double_null()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("Average(1, Value)", new { Value = default(double?) }));
}
