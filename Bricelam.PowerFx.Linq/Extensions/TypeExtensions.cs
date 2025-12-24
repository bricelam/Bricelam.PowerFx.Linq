#pragma warning disable IDE0130

namespace System;

static class TypeExtensions
{
    public static bool IsNullableStruct(this Type type)
        => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public static Type AsNullable(this Type type)
        => type.IsValueType && !type.IsNullableStruct()
            ? typeof(Nullable<>).MakeGenericType(type)
            : type;
}
