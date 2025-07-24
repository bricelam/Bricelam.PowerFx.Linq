namespace Bricelam.PowerFx.Linq;

/// <summary>
/// Represents errors that occur in the Bricelam.PowerFx.Linq library.
/// </summary>
public class PowerFxLinqException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PowerFxLinqException"/> class.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    public PowerFxLinqException(string message)
        : base(message)
    {
    }
}
