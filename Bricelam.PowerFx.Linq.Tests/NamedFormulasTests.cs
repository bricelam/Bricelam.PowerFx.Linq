using Bricelam.PowerFx.Linq.Test;

namespace Bricelam.PowerFx.Linq;

public class NamedFormulasTests
{
    [Fact]
    public void Load_works()
    {
        using var file = new TestFile(
            """
            Formula1: =1
            Formula2: |
              =2
            """);

        var formulas = NamedFormulas.Load(file);

        Assert.Equal(2, formulas.Count);
        Assert.Equal("1", formulas["Formula1"]);
        Assert.Equal("2", formulas["Formula2"]);
    }

    [Fact]
    public void Load_throws_when_no_equal()
    {
        using var file = new TestFile(
            """
            Formula: 0
            """);

        Assert.Throws<PowerFxLinqException>(
            () => NamedFormulas.Load(file));
    }

    [Fact]
    public void GetDependencies_works()
    {
        var formula = "2 * Pi() * R";

        var dependencies = NamedFormulas.GetDependencies(formula);

        Assert.Equal(["R"], dependencies);
    }

    [Fact]
    public void GetDependencies_throws_when_errors()
    {
        var formula = ">= 25";

        var ex = Assert.Throws<PowerFxLinqException>(
            () => NamedFormulas.GetDependencies(formula));

        Assert.Contains("Expected an operand.", ex.Message);
    }
}