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

        var namedFormulas = NamedFormulas.Load(file);

        Assert.Equal(2, namedFormulas.Count);
        Assert.Equal("1", namedFormulas["Formula1"]);
        Assert.Equal("2", namedFormulas["Formula2"]);
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
}
