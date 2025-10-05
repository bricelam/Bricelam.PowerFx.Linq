#pragma warning disable IDE0130

using System.Reflection;

namespace System;

static class TypeExtensions
{
    public static IEnumerable<MethodInfo> GetMethods(this Type type, string name)
        => type.GetMethods().Where(m => m.Name == name);

    public static bool IsNullable(this Type type)
        => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
}
