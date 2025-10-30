using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Bricelam.PowerFx.Linq.Reflection;

abstract class PropertyProvider
{
    static readonly Type _dictionaryType = typeof(Dictionary<string, object?>);
    static readonly MethodInfo _queryableSelectMethod = Enumerable.First(
        from m in typeof(Queryable).GetMethods()
        where m.Name == nameof(Queryable.Select)
        let parameters = m.GetParameters()
        where parameters.Length == 2
            && parameters[0].ParameterType.IsConstructedGenericType
            && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
            && parameters[1].ParameterType.IsConstructedGenericType
            && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
            && parameters[1].ParameterType.GenericTypeArguments[0].IsConstructedGenericType
            && parameters[1].ParameterType.GenericTypeArguments[0].GetGenericTypeDefinition() == typeof(Func<,>)
        select m);

    public static PropertyProvider Create(Type type, Expression? expression)
    {
        var properties = new Dictionary<string, Type>();
        if (type == _dictionaryType)
        {
            if (expression is MethodCallExpression methodCallExpression
                && methodCallExpression.Method.IsConstructedGenericMethod
                && methodCallExpression.Method.GetGenericMethodDefinition() == _queryableSelectMethod)
            {
                var quotedSelector = (UnaryExpression)methodCallExpression.Arguments[1];
                Debug.Assert(quotedSelector.NodeType == ExpressionType.Quote);
                var selector = (LambdaExpression)quotedSelector.Operand;
                var listInit = (ListInitExpression)selector.Body;

                foreach (ElementInit initializer in listInit.Initializers)
                {
                    var keyExpression = (ConstantExpression)initializer.Arguments[0];

                    var valueExpression = (UnaryExpression)initializer.Arguments[1];
                    Debug.Assert(valueExpression.NodeType == ExpressionType.Convert);
                    Debug.Assert(valueExpression.Type == typeof(object));

                    properties.Add((string)keyExpression.Value!, valueExpression.Operand.Type);
                }
            }
            else if (expression is ConstantExpression constantExpression
                && constantExpression.Value is IEnumerable<Dictionary<string, object?>> source)
            {
                var keysWithUnknownType = new List<string>();

                using var enumerator = source.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    throw new PowerFxLinqException("Cannot determine the dictionary keys of the query source.");
                }

                foreach (var property in enumerator.Current)
                {
                    if (property.Value is null)
                    {
                        keysWithUnknownType.Add(property.Key);
                        continue;
                    }

                    properties.Add(property.Key, property.Value.GetType().AsNullable());
                }

                while (keysWithUnknownType.Count != 0 && enumerator.MoveNext())
                {
                    var item = enumerator.Current;

                    for (var i = keysWithUnknownType.Count - 1; i >= 0; i--)
                    {
                        var key = keysWithUnknownType[i];

                        var value = item[key];
                        if (value is not null)
                        {
                            // NB: Assumes all columns could contain null
                            properties.Add(key, value.GetType().AsNullable());
                            keysWithUnknownType.Remove(key);
                        }
                    }
                }

                foreach (var key in keysWithUnknownType)
                {
                    properties.Add(key, typeof(object));
                }
            }
            else
            {
                // TODO: Follow callchain back to the source dictionary
                throw new PowerFxLinqException("Cannot determine the dictionary keys of the query source.");
            }

            return new DictionaryPropertyProvider(properties);
        }

        return new TypePropertyProvider(type);
    }

    public abstract IEnumerable<ProvidedProperty> GetProperties();
    public abstract ProvidedProperty? GetProperty(string name);

    class TypePropertyProvider : PropertyProvider
    {
        readonly Type _type;

        public TypePropertyProvider(Type type)
            => _type = type;

        public override IEnumerable<ProvidedProperty> GetProperties()
            => _type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(p => new TypeProvidedProperty(p));

        public override ProvidedProperty? GetProperty(string name)
        {
            var property = _type.GetProperty(name);

            return property is null ? null : new TypeProvidedProperty(property);
        }

        class TypeProvidedProperty : ProvidedProperty
        {
            readonly PropertyInfo _property;

            public TypeProvidedProperty(PropertyInfo property)
                => _property = property;

            public override string Name
                => _property.Name;

            public override Type PropertyType
                => _property.PropertyType;

            public override Expression CreateAccessExpression(ParameterExpression parameter)
                => Expression.Property(parameter, _property);
        }
    }

    class DictionaryPropertyProvider : PropertyProvider
    {
        static readonly PropertyInfo _itemProperty = _dictionaryType.GetProperty("Item")!;

        readonly Dictionary<string, Type> _properties;

        public DictionaryPropertyProvider(Dictionary<string, Type> properties)
            => _properties = properties;

        public override IEnumerable<ProvidedProperty> GetProperties()
            => _properties.Select(i => new DictionaryProvidedProperty((i.Key, i.Value)));

        public override ProvidedProperty? GetProperty(string name)
            => _properties.TryGetValue(name, out var type)
                ? new DictionaryProvidedProperty((name, type))
                : null;

        class DictionaryProvidedProperty : ProvidedProperty
        {
            readonly (string Key, Type Type) _property;

            public DictionaryProvidedProperty((string Key, Type Type) property)
                => _property = property;

            public override string Name
                => _property.Key;

            public override Type PropertyType
                => _property.Type;

            public override Expression CreateAccessExpression(ParameterExpression parameter)
                => ExpressionExtensions.ConvertIfNeeded(
                    Expression.Property(parameter, _itemProperty, Expression.Constant(_property.Key)),
                    _property.Type);
        }
    }
}
