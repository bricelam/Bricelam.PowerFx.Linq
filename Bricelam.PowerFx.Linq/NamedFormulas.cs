using Microsoft.PowerFx;
using YamlDotNet.Serialization;

namespace Bricelam.PowerFx.Linq;

public static class NamedFormulas
{
    public static Dictionary<string, string> Load(string path)
    {
        using var reader = File.OpenText(path);

        var deserializer = new DeserializerBuilder()
            .Build();

        var formulas = new Dictionary<string, string>();

        foreach (var (key, value) in deserializer.Deserialize<IDictionary<string, string>>(reader))
        {
            if (!value.StartsWith('='))
            {
                throw new PowerFxException($"Named formula '{key}' must begin with a leading equal sign. File: {path}");
            }

            formulas.Add(key, value.Substring(1));
        }

        return formulas;
    }

    public static IEnumerable<string> GetDependencies(string formula)
    {
        var engine = new Engine();
        var parseResult = engine.Parse(formula);
        parseResult.ThrowOnErrors();

        var visitor = new DependencyDiscoveringVisitor();
        parseResult.Root.Accept(visitor);

        return visitor.Dependencies;
    }
}
