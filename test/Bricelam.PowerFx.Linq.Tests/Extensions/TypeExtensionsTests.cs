#pragma warning disable IDE0130

namespace System;

public class TypeExtensionsTests
{
    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int?), true)]
    [InlineData(typeof(object), false)]
    [InlineData(typeof(string), false)]
    public void IsNullableStruct_works(Type type, bool expected)
        => Assert.Equal(expected, type.IsNullableStruct());

    [Theory]
    [InlineData(typeof(int), typeof(int?))]
    [InlineData(typeof(int?), typeof(int?))]
    [InlineData(typeof(object), typeof(object))]
    [InlineData(typeof(string), typeof(string))]
    public void AsNullable_works(Type type, Type expected)
        => Assert.Equal(expected, type.AsNullable());
}
