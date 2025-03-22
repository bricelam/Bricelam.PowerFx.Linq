using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Bricelam.PowerFx.Linq.Translators;
using Microsoft.PowerFx.Syntax;

namespace Bricelam.PowerFx.Linq;

// TODO: Singleton?
class LinqTranslatingVisitor : TexlFunctionalVisitor<Expression, PowerFxExpressionContext>
{
    static readonly Type _dictionaryType = typeof(Dictionary<string, object?>);
    static readonly MethodInfo _dictionaryAddMethod = _dictionaryType
        .GetMethod(nameof(Dictionary<string, object?>.Add), [typeof(string), typeof(object)])!;
    static readonly Type _listType = typeof(List<Dictionary<string, object?>>);
    static readonly MethodInfo _listAddMethod = _listType
        .GetMethod(nameof(List<Dictionary<string, object?>>.Add), [typeof(Dictionary<string, object?>)])!;

    readonly List<IFunctionCallTranslator> _translators = [
        new SimpleBinaryOperatorsTranslator(),
        new SimpleInstanceMethodsTranslator(),
        new SimpleInstancePropertiesTranslator(),
        new SimpleConstantsTranslator(),
        new SimpleStaticMethodsTranslator(),
        new SimpleStaticPropertiesTranslator()
    ];

    public override Expression Visit(ErrorNode node, PowerFxExpressionContext context)
        => throw new PowerFxException(node.Message);

    public override Expression Visit(BlankNode node, PowerFxExpressionContext context)
        => throw new PowerFxException("The formula is blank.");

    public override Expression Visit(BoolLitNode node, PowerFxExpressionContext context)
        => Expression.Constant(node.Value);

    public override Expression Visit(StrLitNode node, PowerFxExpressionContext context)
        => Expression.Constant(node.Value);

    public override Expression Visit(NumLitNode node, PowerFxExpressionContext context)
        => Expression.Constant(node.ActualNumValue);

    public override Expression Visit(DecLitNode node, PowerFxExpressionContext context)
        => Expression.Constant(node.ActualDecValue);

    // TODO: Move logic to PowerFxExpressionContext
    public override Expression Visit(FirstNameNode node, PowerFxExpressionContext context)
    {
        if (context.ThisRecord is not null)
        {
            if (node.Ident.Name == "ThisRecord")
            {
                return context.ThisRecord;
            }

            var property = context.ThisRecord.Type.GetProperty(node.Ident.Name);
            if (property is not null)
            {
                // TODO: Lift all numbers to decimal?
                return Expression.Property(context.ThisRecord, property);
            }
        }

        // TODO: Color.*
        throw new PowerFxException("Unknown identifier: " + node.Ident.Name);
    }

    public override Expression Visit(ParentNode node, PowerFxExpressionContext context)
        => throw new NotImplementedException();

    public override Expression Visit(SelfNode node, PowerFxExpressionContext context)
        => throw new NotImplementedException();

    public override Expression Visit(StrInterpNode node, PowerFxExpressionContext context)
        => Expression.Call(
            typeof(string).GetMethod(nameof(string.Concat), [typeof(object[])])!,
            Expression.NewArrayInit(
                typeof(object),
                node.ChildNodes.Select(n => Expression.Convert(n.Accept(this, context), typeof(object)))));

    public override Expression Visit(DottedNameNode node, PowerFxExpressionContext context)
        // TODO: Handle records
        => Expression.Property(node.Left.Accept(this, context), node.Right.Name);

    public override Expression Visit(UnaryOpNode node, PowerFxExpressionContext context)
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
            return Expression.Multiply(child, Expression.Constant(0.01m));
        }

        throw new UnreachableException("Unexpected UnaryOp: " + node.Op);
    }

    public override Expression Visit(BinaryOpNode node, PowerFxExpressionContext context)
    {
        // TODO: Handle Nullable (add .Value)
        var left = node.Left.Accept(this, context);
        var right = node.Right.Accept(this, context);

        Func<Expression, Expression, Expression>? expressionFactory = node.Op switch
        {
            BinaryOp.Or => Expression.OrElse,
            BinaryOp.And => Expression.AndAlso,
            BinaryOp.Mul => Expression.Multiply,
            BinaryOp.Div => Expression.Divide,
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
            if (left.Type != right.Type)
            {
                // TODO: Better type lifting
                right = Expression.Convert(right, left.Type);
            }

            return expressionFactory(left, right);
        }

        switch (node.Op)
        {
            case BinaryOp.Add:
                return right is UnaryExpression { NodeType: ExpressionType.Negate } negateExpression
                    ? Expression.Subtract(left, negateExpression.Operand)
                    : Expression.Add(left, right);

            case BinaryOp.Concat:
                return Expression.Add(
                    left,
                    right,
                    typeof(string).GetMethod(nameof(string.Concat), [typeof(string), typeof(string)]));

            case BinaryOp.Power:
                return Expression.Convert(
                    Expression.Call(
                        instance: null,
                        typeof(Math).GetMethod(nameof(Math.Pow))!,
                        Expression.Convert(left, typeof(double)),
                        Expression.Convert(right, typeof(double))),
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

    public override Expression Visit(VariadicOpNode node, PowerFxExpressionContext context)
        => throw new NotImplementedException();

    // TODO: Break into more translators
    public override Expression Visit(CallNode node, PowerFxExpressionContext context)
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

        // TODO: DateAdd, DateDiff, ISOWeekNum, IsUTCToday, Replace, Switch, Trim, WeekNum, Average, Sum, StdevP, VarP, Boolean, Char, UniChar, RGBA, ColorFade, Date, Time, DateTime, DateValue, TimeValue, Dec2Hex, Hex2Dec
        // TODO: Got here https://learn.microsoft.com/en-us/power-platform/power-fx/reference/function-isnumeric
        switch (node.Head.Name)
        {
            case "Char":
                return Expression.Call(
                    Expression.Convert(arguments.Single(), typeof(char)),
                    typeof(char).GetMethod(nameof(char.ToString), Type.EmptyTypes)!);

            case "Cot":
                return Expression.Divide(
                    Expression.Constant(1.0),
                    Expression.Call(typeof(Math).GetMethod(nameof(Math.Tan), [typeof(double)])!,
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
                                Expression.Convert(arguments[1], typeof(int)),
                                Expression.Constant(1)),
                            arguments[2]
                    ]);

            case "Not":
                return Expression.Not(arguments.Single());

            case "Right":
                return Expression.Call(
                    arguments[0],
                    typeof(string).GetMethod(nameof(string.Substring), [typeof(int)])!,
                    [
                        // TODO: Protect against negative
                        Expression.Subtract(
                                Expression.Property(arguments[0], nameof(string.Length)),
                                Expression.Convert(arguments[1], typeof(int)))
                    ]);

            case "UTCToday":
                return Expression.Property(
                    Expression.Property(null, typeof(DateTime).GetProperty(nameof(DateTime.UtcNow))!),
                    nameof(DateTime.Date));
        }

        throw new NotImplementedException();
    }

    public override Expression Visit(ListNode node, PowerFxExpressionContext context)
        => throw new NotImplementedException();

    public override Expression Visit(RecordNode node, PowerFxExpressionContext context)
    {
        var initializers = new ElementInit[node.Count];
        for (var i = 0; i < node.Count; i++)
        {
            initializers[i] = Expression.ElementInit(
                _dictionaryAddMethod,
                Expression.Constant(node.Ids[i].Name.Value),
                Expression.Convert(
                    node.ChildNodes[i].Accept(this, context),
                    typeof(object)));
        }

        // TODO: Consider ValueTuple more. Maybe a PowerFx-specific type?
        return Expression.ListInit(
            Expression.New(_dictionaryType),
            initializers);
    }

    public override Expression Visit(TableNode node, PowerFxExpressionContext context)
    {
        var initializers = new List<Expression>();
        foreach (var initalizer in node.ChildNodes.Select(n => n.Accept(this, context)))
        {
            if (initalizer.Type == typeof(Dictionary<string, object?>))
            {
                initializers.Add(initalizer);
                continue;
            }

            // TODO: Use a reducible node to avoid deconstruction later?
            initializers.Add(
                Expression.ListInit(
                    Expression.New(_dictionaryType),
                    Expression.ElementInit(
                        _dictionaryAddMethod,
                        Expression.Constant("Value"),
                        Expression.Convert(
                            initalizer,
                            typeof(object)))));
        }

        return Expression.ListInit(
            Expression.New(_listType),
            _listAddMethod,
            initializers);
    }

    public override Expression Visit(AsNode node, PowerFxExpressionContext context)
        => throw new NotImplementedException();
}
