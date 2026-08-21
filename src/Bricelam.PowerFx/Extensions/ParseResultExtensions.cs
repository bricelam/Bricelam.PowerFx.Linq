#pragma warning disable IDE0130

namespace Microsoft.PowerFx;

/// <summary>
/// Provides <see cref="ParseResult"/> extension methods.
/// </summary>
public static class ParseResultExtensions
{
    /// <summary>
    /// Throws if <see cref="ParseResult.IsSuccess"/> is false.
    /// </summary>
    /// <param name="parseResult">The result to check.</param>
    public static void ThrowOnErrors(this ParseResult parseResult)
    {
        if (!parseResult.IsSuccess)
        {
            throw new InvalidOperationException("Errors: " + string.Join(Environment.NewLine, parseResult.Errors));
        }
    }
}
