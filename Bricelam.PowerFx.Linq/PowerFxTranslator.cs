using System.Diagnostics;
using System.Linq.Expressions;
using Bricelam.PowerFx.Linq.Expressions;
using Bricelam.PowerFx.Linq.Translators;
using Microsoft.PowerFx.Syntax;

namespace Bricelam.PowerFx.Linq;

// TODO: Singleton?
class PowerFxTranslator : TexlFunctionalVisitor<Expression, PowerFxTranslatorContext>
{
    readonly List<IFunctionCallTranslator> _translators = [
        new SimpleBinaryOperatorsTranslator(),
        new SimpleConstantsTranslator(),
        new SimpleInstanceMethodsTranslator(),
        new SimpleInstancePropertiesTranslator(),
        new SimpleStaticMethodsTranslator(),
        new SimpleStaticPropertiesTranslator(),
        new SimpleUnaryOperatorsTranslator()
    ];

    public override Expression Visit(TypeLiteralNode node, PowerFxTranslatorContext context)
        => throw new NotImplementedException();

    public override Expression Visit(ErrorNode node, PowerFxTranslatorContext context)
        => throw new PowerFxLinqException(node.Message);

    public override Expression Visit(BlankNode node, PowerFxTranslatorContext context)
        => throw new PowerFxLinqException("The formula is blank.");

    public override Expression Visit(BoolLitNode node, PowerFxTranslatorContext context)
        => Expression.Constant(node.Value);

    public override Expression Visit(StrLitNode node, PowerFxTranslatorContext context)
        => Expression.Constant(node.Value);

    public override Expression Visit(NumLitNode node, PowerFxTranslatorContext context)
        => Expression.Constant(node.ActualNumValue);

    public override Expression Visit(DecLitNode node, PowerFxTranslatorContext context)
        => Expression.Constant(node.ActualDecValue);

    public override Expression Visit(FirstNameNode node, PowerFxTranslatorContext context)
    {
        var translation = context.Bind(node.Ident.Name);
        if (translation is not null)
        {
            return translation;
        }

        // TODO: Handle Color.*
        throw new PowerFxLinqException("Unknown identifier: " + node.Ident.Name);
    }

    public override Expression Visit(ParentNode node, PowerFxTranslatorContext context)
        => throw new NotImplementedException();

    public override Expression Visit(SelfNode node, PowerFxTranslatorContext context)
        => throw new NotImplementedException();

    public override Expression Visit(StrInterpNode node, PowerFxTranslatorContext context)
        => Expression.Call(
            typeof(string).GetMethod(nameof(string.Concat), [typeof(object[])])!,
            Expression.NewArrayInit(
                typeof(object),
                node.ChildNodes.Select(n => Expression.Convert(n.Accept(this, context), typeof(object)))));

    public override Expression Visit(DottedNameNode node, PowerFxTranslatorContext context)
        // TODO: Handle records
        => Expression.Property(node.Left.Accept(this, context), node.Right.Name);

    public override Expression Visit(UnaryOpNode node, PowerFxTranslatorContext context)
    {
        var child = node.Child.Accept(this, context);
        Func<Expression, Expression>? expressionFactory = node.Op switch
        {
            UnaryOp.Not => Expression.Not,
            UnaryOp.Minus => Expression.Negate,
            _ => null
        };
        if (expressionFactory is not null)
        {
            return expressionFactory(child);
        }

        if (node.Op == UnaryOp.Percent)
        {
            return Expression.Divide(
                child,
                ExpressionExtensions.ConvertIfNeeded(
                    Expression.Constant(100.0),
                    child.Type));
        }

        throw new UnreachableException("Unexpected UnaryOp: " + node.Op);
    }

    public override Expression Visit(BinaryOpNode node, PowerFxTranslatorContext context)
    {
        var left = node.Left.Accept(this, context);
        var right = node.Right.Accept(this, context);

        Func<Expression, Expression, Expression>? expressionFactory = node.Op switch
        {
            BinaryOp.Or => Expression.OrElse,
            BinaryOp.And => Expression.AndAlso,
            BinaryOp.Mul => ExpressionExtensions.LiftAndMultiply,
            BinaryOp.Div => ExpressionExtensions.LiftAndDivide,
            BinaryOp.Equal => Expression.Equal,
            BinaryOp.NotEqual => Expression.NotEqual,
            BinaryOp.Less => Expression.LessThan,
            BinaryOp.LessEqual => Expression.LessThanOrEqual,
            BinaryOp.Greater => Expression.GreaterThan,
            BinaryOp.GreaterEqual => Expression.GreaterThanOrEqual,
            _ => null
        };
        if (expressionFactory is not null)
        {
            return expressionFactory(left, right);
        }

        switch (node.Op)
        {
            case BinaryOp.Add:
                return right is UnaryExpression { NodeType: ExpressionType.Negate } negateExpression
                    ? ExpressionExtensions.LiftAndSubtract(left, negateExpression.Operand)
                    : ExpressionExtensions.LiftAndAdd(left, right);

            case BinaryOp.Concat:
                return Expression.Add(
                    left,
                    right,
                    typeof(string).GetMethod(nameof(string.Concat), [typeof(string), typeof(string)]));

            case BinaryOp.Power:
                return ExpressionExtensions.ConvertIfNeeded(
                    Expression.Call(
                        instance: null,
                        typeof(Math).GetMethod(nameof(Math.Pow))!,
                        ExpressionExtensions.ConvertIfNeeded(left, typeof(double)),
                        ExpressionExtensions.ConvertIfNeeded(right, typeof(double))),
                    left.Type);

            case BinaryOp.In: // TODO: Case-insensitive
            case BinaryOp.Exactin:
                return left.Type == typeof(string) && right.Type == typeof(string)
                    ? Expression.Call(
                        right,
                        typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                        left)
                    // TODO: Handle lists
                    : throw new NotImplementedException();
        }

        throw new UnreachableException("Unexpected BinaryOp: " + node.Op);
    }

    public override Expression Visit(VariadicOpNode node, PowerFxTranslatorContext context)
        => throw new NotImplementedException();

    // TODO: Break into more translators
    public override Expression Visit(CallNode node, PowerFxTranslatorContext context)
    {
        var arguments = node.Args.ChildNodes.Select(c => c.Accept(this, context)).ToList();

        foreach (var translator in _translators)
        {
            var translation = translator.Translate(node.Head.Name, arguments);
            if (translation is not null)
            {
                return translation;
            }
        }

        switch (node.Head.Name)
        {
            case "Acot":
                return Expression.Subtract(
                    Expression.Divide(
                        Expression.Constant(Math.PI),
                        Expression.Constant(2.0)),
                    Expression.Call(
                        typeof(Math).GetMethod(nameof(Math.Atan), [typeof(double)])!,
                        arguments));

            case "Average":
                return ExpressionExtensions.LiftAndDivide(
                    SimpleBinaryOperatorsTranslator.CreateBinaryTree(
                        ExpressionExtensions.LiftAndAdd,
                        arguments),
                    Expression.Constant((double)arguments.Count));

            case "Char":
                return Expression.Call(
                    Expression.Convert(arguments.Single(), typeof(char)),
                    typeof(char).GetMethod(nameof(char.ToString), Type.EmptyTypes)!);

            case "Cot":
                return Expression.Divide(
                    Expression.Constant(1.0),
                    Expression.Call(
                        typeof(Math).GetMethod(nameof(Math.Tan), [typeof(double)])!,
                        arguments));

            case "GUID":
                return Expression.Call(
                    arguments.Count == 0
                        ? typeof(Guid).GetMethod(nameof(Guid.NewGuid), Type.EmptyTypes)!
                        : typeof(Guid).GetMethod(nameof(Guid.Parse), [typeof(string)])!,
                    arguments);

            case "If":
                // TODO: Handle additional conditions
                return Expression.Condition(arguments[0], arguments[1], arguments[2]);

            // TODO: Handle empty strings
            case "IsBlank":
                var argument = arguments.Single();
                return Expression.Equal(argument, Expression.Constant(null, argument.Type));

            case "Left":
                return Expression.Call(
                    arguments[0],
                    typeof(string).GetMethod(nameof(string.Substring), [typeof(int), typeof(int)])!,
                    [
                        Expression.Constant(0),
                        arguments[1]
                    ]);

            case "Log":
                return Expression.Call(
                    arguments.Count == 1
                        ? typeof(Math).GetMethod(nameof(Math.Log10), [typeof(double)])!
                        : typeof(Math).GetMethod(nameof(Math.Log), [typeof(double), typeof(double)])!,
                    arguments);

            case "Mid":
                return Expression.Call(
                    arguments[0],
                    typeof(string).GetMethod(nameof(string.Substring), [typeof(int), typeof(int)])!,
                    [
                        Expression.Subtract(
                                ExpressionExtensions.ConvertIfNeeded(arguments[1], typeof(int)),
                                Expression.Constant(1)),
                            arguments[2]
                    ]);

            case "Right":
                return Expression.Call(
                    arguments[0],
                    typeof(string).GetMethod(nameof(string.Substring), [typeof(int)])!,
                    [
                        // TODO: Protect against negative
                        Expression.Subtract(
                                Expression.Property(arguments[0], nameof(string.Length)),
                                ExpressionExtensions.ConvertIfNeeded(arguments[1], typeof(int)))
                    ]);

            case "UTCToday":
                return Expression.Property(
                    Expression.Property(null, typeof(DateTime).GetProperty(nameof(DateTime.UtcNow))!),
                    nameof(DateTime.Date));
        }

        throw new NotImplementedException();
    }

    public override Expression Visit(ListNode node, PowerFxTranslatorContext context)
        => throw new NotImplementedException();

    public override Expression Visit(RecordNode node, PowerFxTranslatorContext context)
    {
        var fields = new Dictionary<string, Expression>();
        for (var i = 0; i < node.Count; i++)
        {
            fields.Add(node.Ids[i].Name, node.ChildNodes[i].Accept(this, context));
        }

        return new RecordExpression(fields);
    }

    public override Expression Visit(TableNode node, PowerFxTranslatorContext context)
        => new TableExpression(
            node.ChildNodes.Select(c => c.Accept(this, context))
                .Select(
                    c => c is RecordExpression recordExpression
                        ? recordExpression
                        : RecordExpression.FromValue(c))
                .ToArray());

    public override Expression Visit(AsNode node, PowerFxTranslatorContext context)
        => throw new NotImplementedException();
}
