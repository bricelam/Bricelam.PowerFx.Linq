using YamlDotNet.Serialization;

namespace Bricelam.PowerFx.Linq;

/// <summary>
/// Configures the translation of Power Fx formulas.
/// </summary>
public class PowerFxLinqConfig
{
    static IDeserializer? _yamlDeserializer;

    /// <summary>
    /// Gets a dictionary of named formulas that can be referenced by other formulas during translation.
    /// </summary>
    /// <value>A dictionary of named formulas.</value>
    public Dictionary<string, string> NamedFormulas { get; } = [];

    /// <summary>
    /// <para>Loads named formulas form a YAML formula file.</para>
    /// </summary>
    /// <param name="path">The file to load from.</param>
    /// <seealso href="https://learn.microsoft.com/power-platform/power-fx/yaml-formula-grammar">Power Fx YAML formula grammar</seealso>
    // TODO: This may not bleong here
    public void LoadNamedFormulas(string path)
    {
        using var reader = File.OpenText(path);

        _yamlDeserializer ??= new DeserializerBuilder()
            .Build();

        foreach (var (key, value) in _yamlDeserializer.Deserialize<IDictionary<string, string>>(reader))
        {
            if (!value.StartsWith('='))
            {
                throw new PowerFxLinqException(
                    $"Named formula '{key}' must begin with a leading equal sign. File: {path}");
            }

            NamedFormulas.AddOrSet(key, value[1..]);
        }
    }
}
