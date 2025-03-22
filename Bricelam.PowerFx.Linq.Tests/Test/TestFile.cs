namespace Bricelam.PowerFx.Linq.Test;

class TestFile : IDisposable
{
    readonly string _path;

    public TestFile(string contents)
    {
        _path = Path.GetTempFileName();
        File.WriteAllText(_path, contents);
    }

    public static implicit operator string(TestFile value)
        => value._path;

    public void Dispose()
        => File.Delete(_path);
}
