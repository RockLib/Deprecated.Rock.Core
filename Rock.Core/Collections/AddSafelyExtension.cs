using System.Collections.Generic;

namespace Rock.Collections
{
    /// <summary>
    /// Provides a <see cref="AddSafely{TKey,TValue}"/> extension method.
    /// </summary>
    public static class AddSafelyExtension
    {
        /// <summary>
        /// Adds the specified key and value to the dictionary if and only if the
        /// key is not currently present.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source <see cref="Dictionary{TKey,TValue}"/>.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public static void AddSafely<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (!source.ContainsKey(key))
            {
                source.Add(key, value);
            }
        }
    }
}
