namespace Bricelam.PowerFx.Linq;

/// <summary>
/// Represents errors that occur in the Bricelam.PowerFx.Linq library.
/// </summary>
public class PowerFxLinqException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PowerFxLinqException"/> class.
    /// </summary>
    public PowerFxLinqException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerFxLinqException"/> class.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    public PowerFxLinqException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerFxLinqException"/> class.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PowerFxLinqException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
