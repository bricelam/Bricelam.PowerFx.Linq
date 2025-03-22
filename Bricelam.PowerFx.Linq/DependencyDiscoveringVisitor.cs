using Microsoft.PowerFx.Syntax;

namespace Bricelam.PowerFx.Linq;

class DependencyDiscoveringVisitor : TexlVisitor
{
    public ICollection<string> Dependencies { get; } = new HashSet<string>();

    public override void PostVisit(AsNode node)
    {
    }

    public override void PostVisit(BinaryOpNode node)
    {
    }

    public override void Visit(BlankNode node)
    {
    }

    public override void Visit(BoolLitNode node)
    {
    }

    public override void PostVisit(CallNode node)
    {
    }

    public override void Visit(DecLitNode node)
    {
    }

    public override void PostVisit(DottedNameNode node)
    {
    }

    public override void Visit(ErrorNode node)
    {
    }

    public override void Visit(FirstNameNode node)
        => Dependencies.Add(node.Ident.Name);

    public override void PostVisit(ListNode node)
    {
    }

    public override void Visit(NumLitNode node)
    {
    }

    public override void Visit(ParentNode node)
    {
    }

    public override void PostVisit(RecordNode node)
    {
    }

    public override void Visit(SelfNode node)
    {
    }

    public override void PostVisit(StrInterpNode node)
    {
    }

    public override void Visit(StrLitNode node)
    {
    }

    public override void PostVisit(TableNode node)
    {
    }

    public override void PostVisit(UnaryOpNode node)
    {
    }

    public override void PostVisit(VariadicOpNode node)
    {
    }
}