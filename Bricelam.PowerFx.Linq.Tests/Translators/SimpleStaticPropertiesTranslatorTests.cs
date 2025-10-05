namespace Bricelam.PowerFx.Linq.Translators;

public class SimpleStaticPropertiesTranslatorTests : TranslatorTestBase
{
    [Fact]
    public void Now()
        => ActionTest("Now()");

    [Fact]
    public void Today()
        => FuncTest("Today()", DateTime.Today);

    [Fact]
    public void UTCNow()
        => ActionTest(UTCConfig, "UTCNow()");
}
