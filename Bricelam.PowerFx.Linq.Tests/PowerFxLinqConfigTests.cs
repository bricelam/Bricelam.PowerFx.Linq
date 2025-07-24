using Bricelam.PowerFx.Linq.Test;

namespace Bricelam.PowerFx.Linq;

public class PowerFxLinqConfigTests
{
    [Fact]
    public void LoadNamedFormulas_works()
    {
        using var file = new TestFile(
            """
            Formula1: =1
            Formula2: |
              =2
            """);

        var config = new PowerFxLinqConfig();

        config.LoadNamedFormulas(file);

        Assert.Equal(2, config.NamedFormulas.Count);
        Assert.Equal("1", config.NamedFormulas["Formula1"]);
        Assert.Equal("2", config.NamedFormulas["Formula2"]);
    }

    [Fact]
    public void LoadNamedFormulas_throws_when_no_equal()
    {
        using var file = new TestFile(
            """
            Formula: 0
            """);

        var config = new PowerFxLinqConfig();

        Assert.Throws<PowerFxLinqException>(
            () => config.LoadNamedFormulas(file));
    }
}
