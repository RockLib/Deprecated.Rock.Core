using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rock.Reflection;

namespace IsIDictionryOfTToAnythingExtensionTests
{
    public class TheIsIDictionaryOfTToAnythingExtensionMethod
    {
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        [TestCase(typeof(IEnumerable<bool>))]
        [TestCase(typeof(List<double>))]
        public void ReturnsFalseForCompletelyUnrelatedTypes(Type type)
        {
            Assert.That(type.IsIDictionaryOfTToAnything<string>(), Is.False);
        }

        [TestCase(typeof(IDictionary<int, double>))]
        [TestCase(typeof(Dictionary<int, double>))]
        public void ReturnsFalseForTypesWithTheWrongKeyType(Type type)
        {
            Assert.That(type.IsIDictionaryOfTToAnything<string>(), Is.False);
        }

        [TestCase(typeof(Dictionary<string, double>))]
        [TestCase(typeof(StringDictionary<double>))]
        public void ReturnsTrueForImplementorsOfIDictionaryWithMatchingKeyTypes(Type type)
        {
            Assert.That(type.IsIDictionaryOfTToAnything<string>(), Is.True);
        }

        [TestCase(typeof(IDictionary<string, double>))]
        [TestCase(typeof(IStringDictionary<double>))]
        public void ReturnsTrueForIDictionaryWithMatchingKeyTypes(Type type)
        {
            Assert.That(type.IsIDictionaryOfTToAnything<string>(), Is.True);
        }

        private interface IStringDictionary<TValue> : IDictionary<string, TValue>
        {
        }

        private abstract class StringDictionary<TValue> : IStringDictionary<TValue>
        {
            public abstract IEnumerator<KeyValuePair<string, TValue>> GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public abstract void Add(KeyValuePair<string, TValue> item);
            public abstract void Clear();
            public abstract bool Contains(KeyValuePair<string, TValue> item);
            public abstract void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex);
            public abstract bool Remove(KeyValuePair<string, TValue> item);
            public abstract int Count { get; }
            public abstract bool IsReadOnly { get; }
            public abstract bool ContainsKey(string key);
            public abstract void Add(string key, TValue value);
            public abstract bool Remove(string key);
            public abstract bool TryGetValue(string key, out TValue value);
            public abstract TValue this[string key] { get; set; }
            public abstract ICollection<string> Keys { get; }
            public abstract ICollection<TValue> Values { get; }
        }
    }
}
