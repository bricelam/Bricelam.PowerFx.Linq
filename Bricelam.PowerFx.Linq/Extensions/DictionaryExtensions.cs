#pragma warning disable IDE0130

namespace System.Collections.Generic;

static class DictionaryExtensions
{
    public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        if (!dictionary.TryAdd(key, value))
        {
            dictionary[key] = value;
        }
    }
}
