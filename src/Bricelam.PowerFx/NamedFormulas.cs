using YamlDotNet.Serialization;

namespace Bricelam.PowerFx;

/// <summary>
/// Provides methods for loading named formulas from YAML.
/// </summary>
/// <seealso href="https://learn.microsoft.com/power-platform/power-fx/yaml-formula-grammar">Power Fx YAML formula grammar</seealso>
public static class NamedFormulas
{
    static IDeserializer? _yamlDeserializer;

    static IDeserializer YamlDeserializer
        => _yamlDeserializer
            ??= new DeserializerBuilder()
                .Build();

    /// <summary>
    /// <para>Loads named formulas form a YAML formula file.</para>
    /// </summary>
    /// <param name="path">The file to load from.</param>
    /// <returns>A dictonary of named formulas.</returns>
    public static IDictionary<string, string> Load(string path)
    {
        using var reader = File.OpenText(path);

        var namedFormulas = YamlDeserializer.Deserialize<IDictionary<string, string>>(reader);

        return Validate(namedFormulas, path);
    }

    /// <summary>
    /// <para>Loads named formulas form a YAML formula file.</para>
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    /// <returns>A dictonary of named formulas.</returns>
    public static IDictionary<string, string> Load(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);

        return Load(reader);
    }

    /// <summary>
    /// <para>Loads named formulas form a YAML formula file.</para>
    /// </summary>
    /// <param name="reader">The reader to load from.</param>
    /// <returns>A dictonary of named formulas.</returns>
    public static IDictionary<string, string> Load(TextReader reader)
    {
        var namedFormulas = YamlDeserializer.Deserialize<IDictionary<string, string>>(reader);

        return Validate(namedFormulas);
    }

    /// <summary>
    /// <para>Parses named formulas form YAML.</para>
    /// </summary>
    /// <param name="value">The named formulas YAML.</param>
    /// <returns>A dictonary of named formulas.</returns>
    public static IDictionary<string, string> Parse(string value)
    {
        var namedFormulas = YamlDeserializer.Deserialize<IDictionary<string, string>>(value);

        return Validate(namedFormulas);
    }

    static Dictionary<string, string> Validate(IDictionary<string, string> namedFormulas, string? path = null)
    {
        var result = new Dictionary<string, string>();
        foreach (var (key, value) in namedFormulas)
        {
            if (!value.StartsWith('='))
            {
                var message = $"Named formula '{key}' must begin with a leading equal sign.";
                if (path is not null)
                    message += $" File: {path}";

                throw new NamedFormulaSyntaxException(message);
            }

            result.Add(key, value[1..]);
        }

        return result;
    }
}
