using System.Diagnostics;
using System.Text;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Syntax;

namespace Bricelam.PowerFx.Syntax;

/// <summary>
/// Contains factory methods to create various <see cref="TexlNode"/> types.
/// </summary>
/// <remarks>
/// Because these types don't have public constructors, they're created using
/// <see cref="Engine.Parse(string, ParserOptions)"/>.
/// </remarks>
public static class TexlNodeFactory
{
    static readonly Engine _engine = new();

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.StrInterpNode"/>.
    /// </summary>
    /// <param name="childNodes">The list of child nodes.</param>
    /// <returns>The new node.</returns>
    public static StrInterpNode StrInterpNode(IReadOnlyList<TexlNode> childNodes)
    {
        var builder = new StringBuilder();
        var numberIsFloat = false;

        builder.Append("$\"");

        foreach (var child in childNodes)
        {
            if (child is StrLitNode strLitNode)
            {
                builder.Append(StrLitToken.EscapeString(strLitNode.Value));
            }
            else
            {
                builder.Append('{');
                builder.Append(child);
                builder.Append('}');

                numberIsFloat |= NumberIsFloatVisitor.Run(child);
            }
        }

        builder.Append('\"');

        ParserOptions? options = null;
        if (numberIsFloat)
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return (StrInterpNode)Parse(builder.ToString(), options);
    }
    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.DottedNameNode"/>.
    /// </summary>
    /// <param name="left">The left node of the dotted name.</param>
    /// <param name="right">The right identifier of the dotted name.</param>
    /// <returns>The new node.</returns>
    public static DottedNameNode DottedNameNode(TexlNode left, Identifier right)
    {
        var builder = new StringBuilder();

        builder
            .Append(left)
            .Append('.')
            .Append(right.Name);

        ParserOptions? options = null;
        if (NumberIsFloatVisitor.Run(left))
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return (DottedNameNode)Parse(builder.ToString(), options);
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.UnaryOpNode"/>
    /// </summary>
    /// <param name="op">The unary operation operand.</param>
    /// <param name="child">The unary operator.</param>
    /// <returns>The new node.</returns>
    public static UnaryOpNode UnaryOpNode(UnaryOp op, TexlNode child)
    {
        var builder = new StringBuilder();

        switch (op)
        {
            case UnaryOp.Not:
                builder
                    .Append('!')
                    .Append(child);
                break;

            case UnaryOp.Minus:
                builder
                    .Append('-')
                    .Append(child);
                break;

            case UnaryOp.Percent:
                builder
                    .Append(child)
                    .Append('%');
                break;

            default:
                throw new UnreachableException("Unexpected UnaryOp: " + op);
        }

        ParserOptions? options = null;
        if (NumberIsFloatVisitor.Run(child))
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return (UnaryOpNode)Parse(builder.ToString(), options);
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.BinaryOpNode"/>.
    /// </summary>
    /// <param name="left">The left operand of the binary operation.</param>
    /// <param name="op">The binary operator.</param>
    /// <param name="right">The right operand of the binary operation.</param>
    /// <returns>The new node.</returns>
    public static BinaryOpNode BinaryOpNode(TexlNode left, BinaryOp op, TexlNode right)
    {
        var builder = new StringBuilder();

        builder
            .Append(left)
            .Append(
                op switch
                {
                    BinaryOp.Or => "||",
                    BinaryOp.And => "&&",
                    BinaryOp.Concat => "&",
                    BinaryOp.Add => "+",
                    BinaryOp.Mul => "*",
                    BinaryOp.Div => "/",
                    BinaryOp.In => "in",
                    BinaryOp.Exactin => "exactin",
                    BinaryOp.Power => "^",
                    BinaryOp.Equal => "=",
                    BinaryOp.NotEqual => "<>",
                    BinaryOp.Less => "<",
                    BinaryOp.LessEqual => "<=",
                    BinaryOp.Greater => ">",
                    BinaryOp.GreaterEqual => ">=",
                    _ => throw new UnreachableException("Unexpected BinaryOp: " + op)
                })
            .Append(right);

        ParserOptions? options = null;
        if (NumberIsFloatVisitor.Run(left)
            || NumberIsFloatVisitor.Run(right))
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return (BinaryOpNode)Parse(builder.ToString(), options);
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.VariadicOpNode"/>.
    /// </summary>
    /// <param name="op">The variadic operator.</param>
    /// <param name="childNodes">The list of child nodes.</param>
    /// <returns>The new node.</returns>
    public static VariadicOpNode VariadicOpNode(VariadicOp op, IReadOnlyList<TexlNode> childNodes)
    {
        var builder = new StringBuilder();
        var numberIsFloat = false;

        if (op != VariadicOp.Chain)
        {
            throw new UnreachableException("Unexpected VariadicOp: " + op);
        }

        for (var i = 0; i < childNodes.Count; i++)
        {
            var child = childNodes[i];

            builder.Append(child);

            if (i < childNodes.Count - 1)
            {
                builder.Append(';');
            }

            numberIsFloat |= NumberIsFloatVisitor.Run(child);
        }

        var options = _engine.GetDefaultParserOptionsCopy();
        options.AllowsSideEffects = true;
        options.NumberIsFloat = numberIsFloat;

        return (VariadicOpNode)Parse(builder.ToString(), options);
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.CallNode"/>
    /// </summary>
    /// <param name="head">The identifier of the function call.</param>
    /// <param name="args">The argument list of the function call.</param>
    /// <returns>The new node.</returns>
    public static CallNode CallNode(Identifier head, ListNode args)
    {
        var builder = new StringBuilder();

        builder
            .Append(head.Name)
            .Append('(')
            .Append(args)
            .Append(')');

        ParserOptions? options = null;
        if (NumberIsFloatVisitor.Run(args))
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return (CallNode)Parse(builder.ToString(), options);
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.ListNode"/>.
    /// </summary>
    /// <param name="childNodes">The list of child nodes.</param>
    /// <returns>The new node.</returns>
    public static ListNode ListNode(IReadOnlyList<TexlNode> childNodes)
    {
        var builder = new StringBuilder();
        var numberIsFloat = false;

        builder.Append("_TexlNodeFactory__ListNode(");

        for (var i = 0; i < childNodes.Count; i++)
        {
            var child = childNodes[i];

            builder.Append(child);

            if (i < childNodes.Count - 1)
            {
                builder.Append(',');
            }

            numberIsFloat |= NumberIsFloatVisitor.Run(child);
        }

        builder.Append(')');

        ParserOptions? options = null;
        if (numberIsFloat)
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return ((CallNode)Parse(builder.ToString(), options)).Args;
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.RecordNode"/>.
    /// </summary>
    /// <param name="ids">The record field names.</param>
    /// <param name="childNodes">The list of child nodes.</param>
    /// <returns>The new node.</returns>
    /// <exception cref="ArgumentException">
    /// The number of <paramref name="ids"/> and <paramref name="childNodes"/> don't match.
    /// </exception>
    public static RecordNode RecordNode(IReadOnlyList<Identifier> ids, IReadOnlyList<TexlNode> childNodes)
    {
        if (ids.Count != childNodes.Count)
            throw new ArgumentException(
                $"The number of ids ({ids.Count}) and childNodes ({childNodes.Count}) don't match.");

        var builder = new StringBuilder();
        var numberIsFloat = false;

        builder.Append('{');

        for (var i = 0; i < ids.Count; i++)
        {
            var child = childNodes[i];

            builder
                .Append(ids[i].Name)
                .Append(':')
                .Append(child);

            if (i < ids.Count - 1)
            {
                builder.Append(',');
            }

            numberIsFloat |= NumberIsFloatVisitor.Run(child);
        }

        builder.Append('}');

        ParserOptions? options = null;
        if (numberIsFloat)
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return (RecordNode)Parse(builder.ToString(), options);
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.TableNode"/>.
    /// </summary>
    /// <param name="childNodes">The list of child nodes.</param>
    /// <returns>The new node.</returns>
    public static TableNode TableNode(IReadOnlyList<TexlNode> childNodes)
    {
        var builder = new StringBuilder();
        var numberIsFloat = false;

        builder.Append('[');

        for (var i = 0; i < childNodes.Count; i++)
        {
            var child = childNodes[i];

            builder.Append(child);

            if (i < childNodes.Count - 1)
            {
                builder.Append(',');
            }

            numberIsFloat |= NumberIsFloatVisitor.Run(child);
        }

        builder.Append(']');

        ParserOptions? options = null;
        if (numberIsFloat)
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return (TableNode)Parse(builder.ToString(), options);
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.AsNode"/>.
    /// </summary>
    /// <param name="left">The left operand of the as operator.</param>
    /// <param name="right">The identifier of the as operator.</param>
    /// <returns>The new node.</returns>
    public static AsNode AsNode(TexlNode left, Identifier right)
    {
        var builder = new StringBuilder();

        builder
            .Append(left)
            .Append(" As ")
            .Append(right.Name);

        ParserOptions? options = null;
        if (NumberIsFloatVisitor.Run(left))
        {
            options = _engine.GetDefaultParserOptionsCopy();
            options.NumberIsFloat = true;
        }

        return (AsNode)Parse(builder.ToString(), options);
    }

    /// <summary>
    /// Creates a <see cref="Microsoft.PowerFx.Syntax.FirstNameNode"/>.
    /// </summary>
    /// <param name="ident">The identifier of the first name node.</param>
    /// <returns>The new node.</returns>
    public static FirstNameNode FirstNameNode(string ident)
        => (FirstNameNode)Parse(ident);

    static TexlNode Parse(string expressionText, ParserOptions? options = null)
    {
        var parseResult = _engine.Parse(expressionText, options);
        parseResult.ThrowOnErrors();

        return parseResult.Root;
    }
}
