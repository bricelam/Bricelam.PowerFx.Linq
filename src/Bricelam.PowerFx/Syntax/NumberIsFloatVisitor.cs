using Microsoft.PowerFx.Syntax;

namespace Bricelam.PowerFx.Syntax;

sealed class NumberIsFloatVisitor : IdentityTexlVisitor
{
    bool? _numberIsFloat;

    private NumberIsFloatVisitor()
    {
    }

    public static bool Run(TexlNode node)
    {
        var visitor = new NumberIsFloatVisitor();
        node.Accept(visitor);

        return visitor._numberIsFloat
            ?? false;
    }

    public override bool PreVisit(AsNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(TableNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(RecordNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(ListNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(CallNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(BinaryOpNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(UnaryOpNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(DottedNameNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(StrInterpNode node)
        => _numberIsFloat is null;

    public override bool PreVisit(VariadicOpNode node)
        => _numberIsFloat is null;

    public override void Visit(DecLitNode node)
        => _numberIsFloat = false;

    public override void Visit(NumLitNode node)
        => _numberIsFloat = true;
}
