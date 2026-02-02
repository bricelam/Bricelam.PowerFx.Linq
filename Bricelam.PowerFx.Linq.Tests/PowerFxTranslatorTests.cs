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
    public void Constant_number(string formula, double expected)
        => FuncTest(formula, expected);

    [Fact]
    public void Constant_decimal()
        => FuncTest(DecimalConfig, "0.1", 0.1m);

    [Fact]
    public void Identitifier()
        => FuncTest("Value", new { Value = 1.0 }, 1.0);

    [Fact]
    public void Identitifier_this()
        => FuncTest("ThisRecord.Value", new { Value = 1.0 }, 1.0);

    [Fact]
    public void Identitifier_undefined()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("Unknown"));

    [Fact]
    public void String_interpolation()
        => FuncTest("$\"A{1}\"", "A1");

    [Fact]
    public void DottedName()
        => FuncTest("Location.X", new { Location = new { X = 1.0 } }, 1.0);

    [Fact]
    public void Unary_not()
        => FuncTest("!true", false);

    [Fact]
    public void Unary_not_nullable()
        => FuncTest("!Value", new { Value = (bool?)true }, (bool?)false);

    [Fact]
    public void Unary_not_nullable_null()
        => FuncTest("!Value", new { Value = default(bool?) }, default(bool?));

    [Fact]
    public void Unary_minus()
        => FuncTest("-(1)", -1.0);

    [Fact]
    public void Unary_minus_decimal()
        => FuncTest(DecimalConfig, "-(1)", new { Value = 1.0m }, -1.0m);

    [Fact]
    public void Unary_minus_nullable()
        => FuncTest("-Value", new { Value = (double?)1.0 }, (double?)-1.0);

    [Fact]
    public void Unary_minus_nullable_null()
        => FuncTest("-Value", new { Value = default(double?) }, default(double?));

    [Fact]
    public void Unary_percent()
        => FuncTest("1%", 0.01);

    [Fact]
    public void Unary_percent_decimal()
        => FuncTest(DecimalConfig, "1%", 0.01m);

    [Fact]
    public void Unary_percent_nullable()
        => FuncTest("Value%", new { Value = (double?)1.0 }, (double?)0.01);

    [Fact]
    public void Unary_percent_nullable_null()
        => FuncTest("Value%", new { Value = default(double?) }, default(double?));

    [Fact]
    public void Binary_or()
        => FuncTest("false || true", true);

    [Fact]
    public void Binary_or_nullable()
        => FuncTest("Value1 || Value2", new { Value1 = (bool?)false, Value2 = (bool?)true }, (bool?)true);

    [Fact]
    public void Binary_or_nullable_null()
        => FuncTest("Value1 || Value2", new { Value1 = (bool?)false, Value2 = default(bool?) }, default(bool?));

    [Fact]
    public void Binary_and()
        => FuncTest("true && false", false);

    [Fact]
    public void Binary_concat()
        => FuncTest("\"A\" & \"1\"", "A1");

    [Fact]
    public void Binary_add()
        => FuncTest("1 + 1", 2.0);

    [Fact]
    public void Binary_sub()
        => FuncTest("1 - 1", 0.0);

    [Fact]
    public void Binary_mul()
        => FuncTest("1 * 0", 0.0);

    [Fact]
    public void Binary_mul_decimal()
        => FuncTest(DecimalConfig, "1 * Value", new { Value = 0.0m }, 0.0m);

    [Fact]
    public void Binary_mul_decimal_lift()
        => FuncTest("1 * Value", new { Value = 0.0m }, 0.0m);

    [Fact]
    public void Binary_mul_float_lift()
        => FuncTest("1 * Value", new { Value = 0.0f }, 0.0);

    [Fact]
    public void Binary_mul_float_lift_nullable()
        => FuncTest("1 * Value", new { Value = (float?)0.0f }, (double?)0.0);

    [Fact]
    public void Binary_mul_nullable()
        => FuncTest("1 * Value", new { Value = (double?)0.0 }, (double?)0.0);

    [Fact]
    public void Binary_mul_nullable_null()
        => FuncTest("1 * Value", new { Value = default(double?) }, default(double?));

    [Fact]
    public void Binary_div()
        => FuncTest("2 / 2", 1.0);

    [Fact]
    public void Binary_power()
        => FuncTest("2 ^ 3", 8.0);

    [Fact]
    public void Binary_power_nullable()
        => FuncTest("Value ^ 3", new { Value = (double?)2.0 }, 8.0);


    [Fact] // TODO: Should this work?
    public void Binary_power_nullable_null()
        => Assert.Throws<InvalidOperationException>(
            () => ActionTest("Value ^ 3", new { Value = default(double?) }));

    [Fact]
    public void Binary_equal()
        => FuncTest("1 = 1", true);

    [Fact]
    public void Binary_equal_lift()
        => FuncTest("1 = Value", new { Value = 1 }, true);

    [Fact]
    public void Binary_equal_nullable()
        => FuncTest("1 = Value", new { Value = (double?)1.0 }, true);

    [Fact]
    public void Binary_equal_nullable_null()
        => FuncTest("1 = Value", new { Value = default(double?) }, false);

    [Fact]
    public void Binary_not_equal()
        => FuncTest("1 <> 1", false);

    [Fact]
    public void Binary_not_equal_lift()
        => FuncTest("1 <> Value", new { Value = 1 }, false);

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
        => FuncTest("{Value:1}", new Dictionary<string, object?> { { "Value", 1.0 } });

    [Fact]
    public void Table_empty()
        => FuncTest("[]", new List<Dictionary<string, object?>>(0));

    [Fact]
    public void Table_value()
        => FuncTest("[1]", new List<Dictionary<string, object?>> { new() { { "Value", 1.0 } } });

    [Fact]
    public void Table_value_expression()
        => FuncTest("[1 + 1]", new List<Dictionary<string, object?>> { new() { { "Value", 2.0 } } });

    [Fact]
    public void Table_record()
        => FuncTest("[{Value:1}]", new List<Dictionary<string, object?>> { new() { { "Value", 1.0 } } });

    [Fact]
    public void Named_formulas()
    {
        var config = new PowerFxLinqConfig();
        config.NamedFormulas.Add("R", "D / 2");

        FuncTest(config, "2 * Pi() * R", new { D = 2.0 }, 2.0 * Math.PI);
    }

    [Fact]
    public void Call_Average()
        => FuncTest("Average(1, 1)", 1.0);

    [Fact]
    public void Call_Average_one()
        => FuncTest("Average(1)", 1.0);

    [Fact]
    public void Call_Average_many()
        => FuncTest("Average(1, 1, 1)", 1.0);

    [Fact]
    public void Call_Average_double()
        => FuncTest("Average(Value)", new { Value = 1.0 }, 1.0);

    [Fact]
    public void Call_Average_nullable_double()
        => FuncTest("Average(Value)", new { Value = (double?)1.0 }, (double?)1.0);

    [Fact]
    public void Call_Average_nullable_double_null()
        => FuncTest("Average(1, Value)", new { Value = default(double?) }, default(double?));

    [Fact]
    public void Call_Atan2()
        => FuncTest("Atan2(1, 0)", 0.0);

    [Fact]
    public void Call_DateTime()
        => FuncTest("DateTime(2025, 9, 16, 20, 45, 0)", new DateTime(2025, 9, 16, 20, 45, 0));

    [Fact]
    public void Call_DateTime_milliseconds()
        => FuncTest("DateTime(2025, 9, 16, 20, 45, 0, 123)", new DateTime(2025, 9, 16, 20, 45, 0, 123));

    [Fact]
    public void Call_EDate()
        => FuncTest("EDate(Value, 1)", new { Value = new DateTime(2025, 9, 16) }, new DateTime(2025, 10, 16));

    [Fact]
    public void Call_If()
        => FuncTest("If(false, 0, 1)", 1.0);

    [Fact]
    public void Call_If_no_else()
        => FuncTest("If(false, 0)", default(double?));

    [Fact]
    public void Call_If_multiple_conditions()
        => FuncTest("If(false, 0, false, 1, 2)", 2.0);

    [Fact]
    public void Call_If_multiple_conditions_no_else()
        => FuncTest("If(false, 0, false, 1)", default(double?));

    [Fact]
    public void Call_IsBlank_with_nonnullable()
        => FuncTest("IsBlank(1)", false);

    [Fact]
    public void Call_UniChar()
        => FuncTest("UniChar(65)", "A");

    [Fact]
    public void Call_UTCToday()
        => FuncTest(UTCConfig, "UTCToday()", DateTime.UtcNow.Date);

    [Fact]
    public void Call_With()
        => FuncTest("With({Value:1}, Value)", 1.0);

    [Fact]
    public void Call_With_when_nested()
        => FuncTest("With({B:3}, With({C:5}, A * B * C))", new { A = 2.0 }, 30.0);

}
