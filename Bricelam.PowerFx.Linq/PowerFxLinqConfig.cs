namespace Bricelam.PowerFx.Linq;

/// <summary>
/// Configures the translation of Power Fx formulas.
/// </summary>
public class PowerFxLinqConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PowerFxLinqConfig"/> class.
    /// </summary>
    /// <param name="namedFormulas">The initial set of <see cref="NamedFormulas"/>.</param>
    public PowerFxLinqConfig(IDictionary<string, string>? namedFormulas = null)
        => NamedFormulas = namedFormulas is null ? [] : new Dictionary<string, string>(namedFormulas);

    /// <summary>
    /// Gets a dictionary of named formulas that can be referenced by other formulas during translation.
    /// </summary>
    /// <value>A dictionary of named formulas.</value>
    public IDictionary<string, string> NamedFormulas { get; }
}
