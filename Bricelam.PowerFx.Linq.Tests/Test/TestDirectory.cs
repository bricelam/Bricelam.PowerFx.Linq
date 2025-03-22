namespace Bricelam.PowerFx.Linq.Test;

class TestDirectory : IDisposable
{
    readonly DirectoryInfo _directoryInfo = Directory.CreateTempSubdirectory();

    public static implicit operator string(TestDirectory value)
        => value._directoryInfo.FullName;

    public void Dispose()
        => _directoryInfo.Delete(recursive: true);
}
