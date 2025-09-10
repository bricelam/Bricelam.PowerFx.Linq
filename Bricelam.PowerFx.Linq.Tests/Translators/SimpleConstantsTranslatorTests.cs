namespace Bricelam.PowerFx.Linq.Translators;

public class SimpleConstantsTranslatorTests : TranslatorTestBase
{
    [Fact]
    public void Blank()
        => FuncTest("Blank()", default(object));

    [Fact]
    public void Pi()
        => FuncTest("Pi()", Math.PI);
}
