using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq;

// TODO: Parse errors
public class LinqTranslatingVisitorTests
{
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
    public void Constant_Blank()
        => FuncTest("Blank()", default(object));

    [Fact]
    public void Constant_Pi()
        => FuncTest("Pi()", Math.PI);

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
    public void Binary_mul()
        => FuncTest("1 * 0", 0m);

    [Fact]
    public void Binary_power()
        => FuncTest("2 ^ 3", 8m);

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
    public void Table_record()
    => FuncTest("[{Value:1}]", new List<Dictionary<string, object?>> { new() { { "Value", 1m } } });

    static void ActionTest(string formula)
    {
        var expression = PowerFxExpression.Action(formula);

        Expression.Lambda<Action>(expression).Compile().Invoke();
    }

    static void FuncTest<TResult>(string formula, TResult expected)
    {
        var expression = PowerFxExpression.Func<TResult>(formula);

        var actual = expression.Compile().Invoke();

        Assert.Equal(expected, actual);
    }

    // TODO: Test all types
    class TestEntity
    {
        public bool BooleanProperty { get; set; }
        public bool? NullableBooleanProperty { get; set; }
        public System.Drawing.Color ColorProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public DateTime? NullableDateTimeProperty { get; set; }
        public DateTimeOffset DateTimeOffsetProperty { get; set; }
        public DateTimeOffset? NullableDateTimeOffsetProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public decimal? NullableDecimalProperty { get; set; }
        public double DoubleProperty { get; set; }
        public double? NullableDoubleProperty { get; set; }
        public float SingleProperty { get; set; }
        public float? NullableSingleProperty { get; set; }
        public Guid GuidProperty { get; set; }
        public Guid? NullableGuidProperty { get; set; }
        public int Int32Property { get; set; }
        public int? NullableInt32Property { get; set; }
        public long Int64Property { get; set; }
        public long? NullableInt64Property { get; set; }
        public string? StringProperty { get; set; }
        public TimeSpan TimeSpanProperty { get; set; }
        public TimeSpan? NullableTimeSpanProperty { get; set; }
    }
}
