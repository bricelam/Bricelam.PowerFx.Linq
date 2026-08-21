using Microsoft.PowerFx;
using Microsoft.PowerFx.Syntax;

namespace Bricelam.PowerFx.Syntax;

public class TexlNodeFactoryTests
{
    static readonly Engine _engine = new Engine();

    [Fact]
    public void StrInterpNode_works()
    {
        var inputA = (StrLitNode)Parse("\"A\"");
        var inputB = (FirstNameNode)Parse("B");

        var result = TexlNodeFactory.StrInterpNode([inputA, inputB]);

        Assert.Equal(2, result.Count);

        var resultA = Assert.IsType<StrLitNode>(result.ChildNodes[0]);
        Assert.Equal("A", resultA.Value);

        var resultB = Assert.IsType<FirstNameNode>(result.ChildNodes[1]);
        Assert.Equal("B", resultB.Ident.Name);
    }

    [Fact]
    public void DottedNameNode_works()
    {
        var inputA = (FirstNameNode)Parse("A");
        var inputB = ((FirstNameNode)Parse("B")).Ident;

        var result = TexlNodeFactory.DottedNameNode(inputA, inputB);

        var resultA = Assert.IsType<FirstNameNode>(result.Left);
        Assert.Equal("A", resultA.Ident.Name);

        Assert.Equal("B", result.Right.Name);
    }

    [Fact]
    public void UnaryOpNode_works()
    {
        var input1 = (DecLitNode)Parse("1.0");

        var result = TexlNodeFactory.UnaryOpNode(UnaryOp.Minus, input1);

        Assert.Equal(UnaryOp.Minus, result.Op);

        var result1 = Assert.IsType<DecLitNode>(result.Child);
        Assert.Equal(1.0m, result1.ActualDecValue);
    }

    [Fact]
    public void UnaryOpNode_works_when_float()
    {
        var input1 = (NumLitNode)Parse("1.0", o => o.NumberIsFloat = true);

        var result = TexlNodeFactory.UnaryOpNode(UnaryOp.Minus, input1);

        Assert.Equal(UnaryOp.Minus, result.Op);

        var result1 = Assert.IsType<NumLitNode>(result.Child);
        Assert.Equal(1.0, result1.ActualNumValue);
    }

    [Fact]
    public void BinaryOpNode_works()
    {
        var input1 = (DecLitNode)Parse("1.0");
        var input2 = (DecLitNode)Parse("2.0");

        var result = TexlNodeFactory.BinaryOpNode(input1, BinaryOp.Add, input2);

        Assert.Equal(BinaryOp.Add, result.Op);

        var result1 = Assert.IsType<DecLitNode>(result.Left);
        Assert.Equal(1.0m, result1.ActualDecValue);

        var result2 = Assert.IsType<DecLitNode>(result.Right);
        Assert.Equal(2.0m, result2.ActualDecValue);
    }

    [Fact]
    public void BinaryOpNode_works_when_float()
    {
        var input1 = (NumLitNode)Parse("1.0", o => o.NumberIsFloat = true);
        var input2 = (NumLitNode)Parse("2.0", o => o.NumberIsFloat = true);

        var result = TexlNodeFactory.BinaryOpNode(input1, BinaryOp.Add, input2);

        Assert.Equal(BinaryOp.Add, result.Op);

        var result1 = Assert.IsType<NumLitNode>(result.Left);
        Assert.Equal(1.0, result1.ActualNumValue);

        var result2 = Assert.IsType<NumLitNode>(result.Right);
        Assert.Equal(2.0, result2.ActualNumValue);
    }

    [Fact]
    public void VariadicOpNode_works()
    {
        var inputA = (CallNode)Parse("A()");
        var inputB = (CallNode)Parse("B()");

        var result = TexlNodeFactory.VariadicOpNode(VariadicOp.Chain, [inputA, inputB]);

        Assert.Equal(VariadicOp.Chain, result.Op);
        Assert.Equal(2, result.Count);

        var resultA = Assert.IsType<CallNode>(result.ChildNodes[0]);
        Assert.Equal("A", resultA.Head.Name);
        Assert.Equal(0, resultA.Args.Count);

        var resultB = Assert.IsType<CallNode>(result.ChildNodes[1]);
        Assert.Equal("B", resultB.Head.Name);
        Assert.Equal(0, resultA.Args.Count);
    }

    [Fact]
    public void CallNode_works()
    {
        var inputHead = ((FirstNameNode)Parse("X")).Ident;
        var inputArgs = ((CallNode)Parse("_(1.0, 2.0)")).Args;

        var result = TexlNodeFactory.CallNode(inputHead, inputArgs);

        Assert.Equal("X", result.Head.Name);
        Assert.Equal(2, result.Args.Count);

        var resultArg1 = Assert.IsType<DecLitNode>(result.Args.ChildNodes[0]);
        Assert.Equal(1.0m, resultArg1.ActualDecValue);

        var resultArg2 = Assert.IsType<DecLitNode>(result.Args.ChildNodes[1]);
        Assert.Equal(2.0m, resultArg2.ActualDecValue);
    }

    [Fact]
    public void CallNode_works_when_float()
    {
        var inputHead = ((FirstNameNode)Parse("X")).Ident;
        var inputArgs = ((CallNode)Parse("_(1.0, 2.0)", o => o.NumberIsFloat = true)).Args;

        var result = TexlNodeFactory.CallNode(inputHead, inputArgs);

        Assert.Equal("X", result.Head.Name);
        Assert.Equal(2, result.Args.Count);

        var resultArg1 = Assert.IsType<NumLitNode>(result.Args.ChildNodes[0]);
        Assert.Equal(1.0, resultArg1.ActualNumValue);

        var resultArg2 = Assert.IsType<NumLitNode>(result.Args.ChildNodes[1]);
        Assert.Equal(2.0, resultArg2.ActualNumValue);
    }

    [Fact]
    public void ListNode_works()
    {
        var inputArg1 = (DecLitNode)Parse("1.0");
        var inputArg2 = (DecLitNode)Parse("2.0");

        var result = TexlNodeFactory.ListNode([inputArg1, inputArg2]);

        Assert.Equal(2, result.Count);

        var resultArg1 = Assert.IsType<DecLitNode>(result.ChildNodes[0]);
        Assert.Equal(1.0m, resultArg1.ActualDecValue);

        var resultArg2 = Assert.IsType<DecLitNode>(result.ChildNodes[1]);
        Assert.Equal(2.0m, resultArg2.ActualDecValue);
    }

    [Fact]
    public void ListNode_works_when_float()
    {
        var inputArg1 = (NumLitNode)Parse("1.0", o => o.NumberIsFloat = true);
        var inputArg2 = (NumLitNode)Parse("2.0", o => o.NumberIsFloat = true);

        var result = TexlNodeFactory.ListNode([inputArg1, inputArg2]);

        Assert.Equal(2, result.Count);

        var resultArg1 = Assert.IsType<NumLitNode>(result.ChildNodes[0]);
        Assert.Equal(1.0, resultArg1.ActualNumValue);

        var resultArg2 = Assert.IsType<NumLitNode>(result.ChildNodes[1]);
        Assert.Equal(2.0, resultArg2.ActualNumValue);
    }

    [Fact]
    public void RecordNode_works()
    {
        var inputName = ((FirstNameNode)Parse("Value")).Ident;
        var inputValue = (DecLitNode)Parse("1.0");

        var result = TexlNodeFactory.RecordNode([inputName], [inputValue]);

        Assert.Equal(1, result.Count);
        Assert.Equal("Value", result.Ids[0].Name);

        var resultValue = Assert.IsType<DecLitNode>(result.ChildNodes[0]);
        Assert.Equal(1.0m, resultValue.ActualDecValue);
    }

    [Fact]
    public void RecordNode_works_when_float()
    {
        var inputName = ((FirstNameNode)Parse("Value")).Ident;
        var inputValue = (NumLitNode)Parse("1.0", o => o.NumberIsFloat = true);

        var result = TexlNodeFactory.RecordNode([inputName], [inputValue]);

        Assert.Equal(1, result.Count);
        Assert.Equal("Value", result.Ids[0].Name);

        var resultValue = Assert.IsType<NumLitNode>(result.ChildNodes[0]);
        Assert.Equal(1.0, resultValue.ActualNumValue);
    }

    [Fact]
    public void TableNode_works()
    {
        var inputValue = (DecLitNode)Parse("1.0");

        var result = TexlNodeFactory.TableNode([inputValue]);

        Assert.Equal(1, result.Count);

        var resultValue = Assert.IsType<DecLitNode>(result.ChildNodes[0]);
        Assert.Equal(1.0m, resultValue.ActualDecValue);
    }

    [Fact]
    public void TableNode_works_when_float()
    {
        var inputValue = (NumLitNode)Parse("1.0", o => o.NumberIsFloat = true);

        var result = TexlNodeFactory.TableNode([inputValue]);

        Assert.Equal(1, result.Count);

        var resultValue = Assert.IsType<NumLitNode>(result.ChildNodes[0]);
        Assert.Equal(1.0, resultValue.ActualNumValue);
    }

    [Fact]
    public void AsNode_works()
    {
        var inputLeft = (FirstNameNode)Parse("X");
        var inputRight = ((FirstNameNode)Parse("Y")).Ident;

        var result = TexlNodeFactory.AsNode(inputLeft, inputRight);

        var resultLeft = Assert.IsType<FirstNameNode>(result.Left);
        Assert.Equal("X", resultLeft.Ident.Name);

        Assert.Equal("Y", result.Right.Name);
    }

    [Fact]
    public void FirstNameNode_works()
    {
        var result = TexlNodeFactory.FirstNameNode("X");

        Assert.Equal("X", result.Ident.Name);
    }

    static TexlNode Parse(string expressionText, Action<ParserOptions>? setOptions = null)
    {
        ParserOptions? options = null;
        if (setOptions is not null)
        {
            options = _engine.GetDefaultParserOptionsCopy();
            setOptions(options);
        }

        var parseResult = _engine.Parse(expressionText, options);
        parseResult.ThrowOnErrors();

        return parseResult.Root;
    }
}
