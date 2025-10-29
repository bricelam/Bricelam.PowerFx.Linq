#pragma warning disable IDE0130

using System.Diagnostics;

namespace System;

static class TypeExtensions
{
    public static bool IsNullable(this Type type)
        => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public static Type AsNullable(this Type type)
    {
        Debug.Assert(!type.IsNullable());

        return type.IsValueType
            ? typeof(Nullable<>).MakeGenericType(type)
            : type;
    }
}
