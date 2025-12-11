using System.Diagnostics;
using System.Linq.Expressions;
using Bricelam.PowerFx.Linq.Expressions;
using Bricelam.PowerFx.Linq.Translators;
using Microsoft.PowerFx.Syntax;

namespace Bricelam.PowerFx.Linq;

// TODO: Singleton?
class PowerFxTranslator : TexlFunctionalVisitor<Expression, IPowerFxTranslatorContext>
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

    public override Expression Visit(TypeLiteralNode node, IPowerFxTranslatorContext context)
        => throw new NotImplementedException();

    public override Expression Visit(ErrorNode node, IPowerFxTranslatorContext context)
        => throw new PowerFxLinqException(node.Message);

    public override Expression Visit(BlankNode node, IPowerFxTranslatorContext context)
        => throw new PowerFxLinqException("The formula is blank.");

    public override Expression Visit(BoolLitNode node, IPowerFxTranslatorContext context)
        => Expression.Constant(node.Value);

    public override Expression Visit(StrLitNode node, IPowerFxTranslatorContext context)
        => Expression.Constant(node.Value);

    public override Expression Visit(NumLitNode node, IPowerFxTranslatorContext context)
        => Expression.Constant(node.ActualNumValue);

    public override Expression Visit(DecLitNode node, IPowerFxTranslatorContext context)
        => Expression.Constant(node.ActualDecValue);

    public override Expression Visit(FirstNameNode node, IPowerFxTranslatorContext context)
    {
        var translation = context.Bind(node.Ident.Name);
        if (translation is not null)
        {
            return translation;
        }

        // TODO: Handle Color.*
        throw new UnreachableException("Unknown identifier: " + node.Ident.Name);
    }

    public override Expression Visit(ParentNode node, IPowerFxTranslatorContext context)
        => throw new NotImplementedException();

    public override Expression Visit(SelfNode node, IPowerFxTranslatorContext context)
        => throw new NotImplementedException();

    public override Expression Visit(StrInterpNode node, IPowerFxTranslatorContext context)
        // TODO: DRY (Concatenate function). Handle additonal arguments
        => Expression.Call(
            typeof(string).GetMethod(nameof(string.Concat), [typeof(object[])])!,
            Expression.NewArrayInit(
                typeof(object),
                node.ChildNodes.Select(n => Expression.Convert(n.Accept(this, context), typeof(object)))));

    public override Expression Visit(DottedNameNode node, IPowerFxTranslatorContext context)
        // TODO: Handle records
        => Expression.Property(node.Left.Accept(this, context), node.Right.Name);

    public override Expression Visit(UnaryOpNode node, IPowerFxTranslatorContext context)
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
            return ExpressionExtensions.LiftAndDivide(
                child,
                Expression.Constant(100.0));
        }

        throw new UnreachableException("Unexpected UnaryOp: " + node.Op);
    }

    public override Expression Visit(BinaryOpNode node, IPowerFxTranslatorContext context)
    {
        var left = node.Left.Accept(this, context);
        var right = node.Right.Accept(this, context);

        Func<Expression, Expression, Expression>? expressionFactory = node.Op switch
        {
            BinaryOp.Or => Expression.OrElse,
            BinaryOp.And => Expression.AndAlso,
            BinaryOp.Mul => ExpressionExtensions.LiftAndMultiply,
            BinaryOp.Div => ExpressionExtensions.LiftAndDivide,
            BinaryOp.Equal => ExpressionExtensions.LiftAndEqual,
            BinaryOp.NotEqual => ExpressionExtensions.LiftAndNotEqual,
            BinaryOp.Less => ExpressionExtensions.LiftAndLessThan,
            BinaryOp.LessEqual => ExpressionExtensions.LiftAndLessThanOrEqual,
            BinaryOp.Greater => ExpressionExtensions.LiftAndGreaterThan,
            BinaryOp.GreaterEqual => ExpressionExtensions.LiftAndGreaterThanOrEqual,
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

            // TODO: DRY (Power function)
            case BinaryOp.Power:
                return Expression.Call(
                    instance: null,
                    typeof(Math).GetMethod(nameof(Math.Pow), [typeof(double), typeof(double)])!,
                    [
                        ExpressionExtensions.ConvertIfNeeded(left, typeof(double)),
                        ExpressionExtensions.ConvertIfNeeded(right, typeof(double))
                    ]);

            case BinaryOp.In: // TODO: Case-insensitive
            case BinaryOp.Exactin:
                return left.Type == typeof(string) && right.Type == typeof(string)
                    ? Expression.Call(
                        right,
                        typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                        [
                            left
                        ])
                    // TODO: Handle lists
                    : throw new NotImplementedException();
        }

        throw new UnreachableException("Unexpected BinaryOp: " + node.Op);
    }

    public override Expression Visit(VariadicOpNode node, IPowerFxTranslatorContext context)
        => throw new NotImplementedException();

    // TODO: Break into more translators
    public override Expression Visit(CallNode node, IPowerFxTranslatorContext context)
    {
        if (node.Head.Name == "With")
        {
            var record = (RecordExpression)node.Args.ChildNodes[0].Accept(this, context);
            var withContext = new PowerFxTranslatorContextForWith(context.NumberIsDecimal, record.Fields);
            var formulaNode = node.Args.ChildNodes[1];

            // TODO: Can (and should) we translate this closer to let?
            return formulaNode.Accept(this, withContext);
        }

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
            // TODO: Test Nullable, float, and decimal (all trigonomic functions)
            case "Acot":
                return Expression.Subtract(
                    Expression.Divide(
                        Expression.Constant(Math.PI),
                        Expression.Constant(2.0)),
                    Expression.Call(
                        typeof(Math).GetMethod(nameof(Math.Atan), [typeof(double)])!,
                        [
                            ExpressionExtensions.ConvertIfNeeded(arguments[0], typeof(double))
                        ]));

            case "Atan2":
                return Expression.Call(
                    typeof(Math).GetMethod(nameof(Math.Atan2), [typeof(double), typeof(double)])!,
                    [
                        // NB: Arguments are reversed
                        ExpressionExtensions.ConvertIfNeeded(arguments[1], typeof(double)),
                        ExpressionExtensions.ConvertIfNeeded(arguments[0], typeof(double))
                    ]);

            case "Average":
                return ExpressionExtensions.LiftAndDivide(
                    SimpleBinaryOperatorsTranslator.CreateBinaryTree(
                        ExpressionExtensions.LiftAndAdd,
                        arguments),
                    Expression.Constant((double)arguments.Count));

            case "Blank":
                return Expression.Constant(null);

            case "Char":
            case "UniChar":
                return Expression.Call(
                    Expression.Convert(arguments[0], typeof(char)),
                    typeof(char).GetMethod(nameof(char.ToString), Type.EmptyTypes)!);

            case "Cot":
                return Expression.Divide(
                    Expression.Constant(1.0),
                    Expression.Call(
                        typeof(Math).GetMethod(nameof(Math.Tan), [typeof(double)])!,
                        [
                            arguments[0]
                        ]));

            // TODO: Can we use CallBestOverload?
            case "DateTime":
                return arguments.Count == 6
                    ? Expression.New(
                        typeof(DateTime).GetConstructor([typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int)])!,
                        [
                            ExpressionExtensions.ConvertIfNeeded(arguments[0], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[1], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[2], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[3], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[4], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[5], typeof(int))
                        ])
                    : Expression.New(
                        typeof(DateTime).GetConstructor([typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int)])!,
                        [
                            ExpressionExtensions.ConvertIfNeeded(arguments[0], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[1], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[2], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[3], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[4], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[5], typeof(int)),
                            ExpressionExtensions.ConvertIfNeeded(arguments[6], typeof(int))
                        ]);

            case "EDate":
                return Expression.Call(
                    arguments[0],
                    typeof(DateTime).GetMethod(nameof(DateTime.AddMonths), [typeof(int)])!,
                    [
                        ExpressionExtensions.ConvertIfNeeded(arguments[1], typeof(int))
                    ]);

            case "If":
                var stack = new Stack<Expression>(arguments);
                var expression = stack.Count % 2 == 1
                    ? stack.Pop()
                    : Expression.Constant(null);
                while (stack.Count > 0)
                {
                    // NB: We're traversing the arguments backwards
                    var elseResult = expression;
                    var thenResult = stack.Pop();
                    var condition = stack.Pop();

                    expression = ExpressionExtensions.LiftAndCondition(condition, thenResult, elseResult);
                }

                return expression;

            case "IsBlank":
                return arguments[0].Type == typeof(string)
                    ? Expression.Call(
                        typeof(string).GetMethod(nameof(string.IsNullOrEmpty), [typeof(string)])!,
                        [
                            arguments[0]
                        ])
                    : Expression.Equal(arguments[0], Expression.Constant(null, arguments[0].Type));

            case "Left":
                return Expression.Call(
                    arguments[0],
                    typeof(string).GetMethod(nameof(string.Substring), [typeof(int), typeof(int)])!,
                    [
                        Expression.Constant(0),
                        ExpressionExtensions.ConvertIfNeeded(arguments[1], typeof(int))
                    ]);

            // TODO: Handle just two parameters
            case "Mid":
                return Expression.Call(
                    arguments[0],
                    typeof(string).GetMethod(nameof(string.Substring), [typeof(int), typeof(int)])!,
                    [
                        Expression.Subtract(
                            ExpressionExtensions.ConvertIfNeeded(arguments[1], typeof(int)),
                            Expression.Constant(1)),
                        ExpressionExtensions.ConvertIfNeeded(arguments[2], typeof(int))
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

            // TODO: Handle LanguageTag parameter
            case "Value":
                return Expression.Call(
                    context.NumberIsDecimal
                        ? typeof(decimal).GetMethod(nameof(decimal.Parse), [typeof(string)])!
                        : typeof(double).GetMethod(nameof(double.Parse), [typeof(string)])!,
                    [
                        arguments[0]
                    ]);

            case "UTCToday":
                return Expression.Property(
                    Expression.Property(expression: null, typeof(DateTime), nameof(DateTime.UtcNow))!,
                    nameof(DateTime.Date));
        }

        throw new NotImplementedException();
    }

    public override Expression Visit(ListNode node, IPowerFxTranslatorContext context)
        => throw new NotImplementedException();

    public override Expression Visit(RecordNode node, IPowerFxTranslatorContext context)
    {
        var fields = new Dictionary<string, Expression>();
        for (var i = 0; i < node.Count; i++)
        {
            fields.Add(node.Ids[i].Name, node.ChildNodes[i].Accept(this, context));
        }

        return new RecordExpression(fields);
    }

    public override Expression Visit(TableNode node, IPowerFxTranslatorContext context)
        => new TableExpression(
            node.ChildNodes.Select(c => c.Accept(this, context))
                .Select(
                    c => c is RecordExpression recordExpression
                        ? recordExpression
                        : RecordExpression.FromValue(c))
                .ToArray());

    public override Expression Visit(AsNode node, IPowerFxTranslatorContext context)
        => throw new NotImplementedException();
}
