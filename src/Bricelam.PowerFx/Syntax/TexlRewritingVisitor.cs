using Microsoft.PowerFx.Syntax;

namespace Bricelam.PowerFx.Syntax;

/// <summary>
/// Represents a rewriter for syntax trees.
/// </summary>
/// <typeparam name="TContext">The context type of the visitor.</typeparam>
public abstract class TexlRewritingVisitor<TContext> : TexlFunctionalVisitor<TexlNode, TContext>
{
    /// <summary>
    /// Visits the <see cref="TypeLiteralNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(TypeLiteralNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="ErrorNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(ErrorNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="BlankNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(BlankNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="BoolLitNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(BoolLitNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="StrLitNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(StrLitNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="NumLitNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(NumLitNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="DecLitNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(DecLitNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="FirstNameNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(FirstNameNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="ParentNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(ParentNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="SelfNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(SelfNode node, TContext context)
        => node;

    /// <summary>
    /// Visits the <see cref="StrInterpNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(StrInterpNode node, TContext context)
    {
        var modified = false;

        var childNodes = new List<TexlNode>();
        foreach (var child in node.ChildNodes)
        {
            var newChild = child.Accept(this, context);
            modified |= newChild != child;
            childNodes.Add(newChild);
        }

        if (modified)
        {
            return TexlNodeFactory.StrInterpNode(childNodes);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="DottedNameNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(DottedNameNode node, TContext context)
    {
        var left = node.Left.Accept(this, context);
        if (left != node.Left)
        {
            return TexlNodeFactory.DottedNameNode(left, node.Right);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="UnaryOpNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(UnaryOpNode node, TContext context)
    {
        var child = node.Child.Accept(this, context);
        if (child != node.Child)
        {
            return TexlNodeFactory.UnaryOpNode(node.Op, child);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="BinaryOpNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(BinaryOpNode node, TContext context)
    {
        var left = node.Left.Accept(this, context);
        var right = node.Right.Accept(this, context);
        if (left != node.Left
            || right != node.Right)
        {
            return TexlNodeFactory.BinaryOpNode(left, node.Op, right);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="VariadicOpNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(VariadicOpNode node, TContext context)
    {
        var modified = false;

        var childNodes = new List<TexlNode>();
        foreach (var child in node.ChildNodes)
        {
            var newChild = child.Accept(this, context);
            modified |= newChild != child;
            childNodes.Add(newChild);
        }

        if (modified)
        {
            return TexlNodeFactory.VariadicOpNode(node.Op, childNodes);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="CallNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(CallNode node, TContext context)
    {
        var args = (ListNode)node.Args.Accept(this, context);
        if (args != node.Args)
        {
            return TexlNodeFactory.CallNode(node.Head, args);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="ListNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(ListNode node, TContext context)
    {
        var modified = false;

        var childNodes = new List<TexlNode>();
        foreach (var child in node.ChildNodes)
        {
            var newChild = child.Accept(this, context);
            modified |= newChild != child;
            childNodes.Add(newChild);
        }

        if (modified)
        {
            return TexlNodeFactory.ListNode(childNodes);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="RecordNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(RecordNode node, TContext context)
    {
        var modified = false;

        var childNodes = new List<TexlNode>();
        foreach (var child in node.ChildNodes)
        {
            var newChild = child.Accept(this, context);
            modified |= newChild != child;
            childNodes.Add(newChild);
        }

        if (modified)
        {
            return TexlNodeFactory.RecordNode(node.Ids, childNodes);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="TableNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(TableNode node, TContext context)
    {
        var modified = false;

        var childNodes = new List<TexlNode>();
        foreach (var child in node.ChildNodes)
        {
            var newChild = child.Accept(this, context);
            modified |= newChild != child;
            childNodes.Add(newChild);
        }

        if (modified)
        {
            return TexlNodeFactory.TableNode(childNodes);
        }

        return node;
    }

    /// <summary>
    /// Visits the <see cref="AsNode"/>.
    /// </summary>
    /// <param name="node">The visited node.</param>
    /// <param name="context">The context.</param>
    /// <returns>The modified node if it or any child nodes were modified; otherwise returns the original node.</returns>
    public override TexlNode Visit(AsNode node, TContext context)
    {
        var left = node.Left.Accept(this, context);
        if (left != node.Left)
        {
            return TexlNodeFactory.AsNode(left, node.Right);
        }

        return node;
    }
}
