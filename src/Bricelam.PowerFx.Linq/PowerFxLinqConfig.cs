using Microsoft.PowerFx;

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
    public PowerFxLinqConfig(IEnumerable<KeyValuePair<string, string>>? namedFormulas = null)
        => NamedFormulas = namedFormulas is null ? [] : namedFormulas.ToDictionary();

    /// <summary>
    /// Gets a dictionary of named formulas that can be referenced by other formulas during translation.
    /// </summary>
    /// <value>A dictionary of named formulas.</value>
    public IDictionary<string, string> NamedFormulas { get; }

    /// <summary>
    /// Gets or sets an action to configure the Power Fx compiler.
    /// </summary>
    /// <value>An action to configure the Power Fx compiler.</value>
    public Action<PowerFxConfig>? ConfigureEngine { get; set; }

    /// <summary>
    /// Gets or sets an action to configure the Power Fx parser.
    /// </summary>
    /// <value>An action to configure the Power Fx parser.</value>
    public Action<ParserOptions>? ConfigureParser { get; set; }
}
