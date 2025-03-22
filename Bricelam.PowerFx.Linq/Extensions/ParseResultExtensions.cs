using System.Text;
using Bricelam.PowerFx.Linq;

#pragma warning disable IDE0130

namespace Microsoft.PowerFx;

static class ParseResultExtensions
{
    public static void ThrowOnErrors(this ParseResult parseResult)
    {
        if (parseResult.IsSuccess)
        {
            return;
        }

        var builder = new StringBuilder();

        builder.Append("The formula contains one or more errors:");

        foreach (var error in parseResult.Errors)
        {
            builder
                .AppendLine()
                .Append(error.ToString());
        }

        throw new PowerFxException(builder.ToString());
    }
}
