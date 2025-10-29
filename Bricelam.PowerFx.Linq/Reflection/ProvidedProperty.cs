using System.Linq.Expressions;

namespace Bricelam.PowerFx.Linq.Reflection;

abstract class ProvidedProperty
{
    public abstract string Name { get; }
    public abstract Type PropertyType { get; }

    public abstract Expression CreateAccessExpression(ParameterExpression parameter);
}
