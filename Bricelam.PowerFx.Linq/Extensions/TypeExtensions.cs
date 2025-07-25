#pragma warning disable IDE0130

namespace System;

static class TypeExtensions
{
    public static bool IsNullable(this Type type)
        => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
}
