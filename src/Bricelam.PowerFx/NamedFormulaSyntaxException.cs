namespace Bricelam.PowerFx;

/// <summary>
/// Represents syntax errors that occur when parsing named formulas using <see cref="NamedFormulas"/>.
/// </summary>
public class NamedFormulaSyntaxException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamedFormulaSyntaxException"/> class.
    /// </summary>
    public NamedFormulaSyntaxException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedFormulaSyntaxException"/> class.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    public NamedFormulaSyntaxException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedFormulaSyntaxException"/> class.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NamedFormulaSyntaxException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
