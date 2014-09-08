using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rock.Collections;

namespace DeepEqualityComparerTests
{
    public class TheEqualsMethod
    {
        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        [TestCase("a", "a", true)]
        [TestCase("a", "b", false)]
        [TestCase(MyEnum.Foo, MyEnum.Foo, true)]
        [TestCase(MyEnum.Foo, MyEnum.Bar, false)]
        public void WorksForValues(object lhs, object rhs, bool expected)
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.EqualTo(expected));
        }

        [TestCaseSource("GetIEnumerableTestCases")]
        public void WorksForIEnumerable(object lhs, object rhs, bool expected)
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.EqualTo(expected));
        }

        [Test]
        public void ReturnsTrueForTheSameReference()
        {
            var obj = new object();
            Assert.That(DeepEqualityComparer.Instance.Equals(obj, obj), Is.True);
        }

        [Test]
        public void ReturnsTrueForTwoNulls()
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(null, null), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenTheLeftIsNullAndTheRightIsNot()
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(null, new object()), Is.False);
        }

        [Test]
        public void ReturnsFalseWhenTheRightIsNullAndTheLeftIsNot()
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(new object(), null), Is.False);
        }

        [TestCase("a", null)]
        [TestCase(null, "a")]
        public void ReturnsFalseWhenOnePropertyValueIsNullAndTheOtherIsNotNullForPropertiesOfASealedReferenceType(string lhsValue, string rhsValue)
        {
            var lhs = new Bar { Value = lhsValue };
            var rhs = new Bar { Value = rhsValue };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsFalseWhenTheTwoObjectsHaveDifferentTypes()
        {
            Assert.That(DeepEqualityComparer.Instance.Equals("abc", 123), Is.False);
        }

        [TestCaseSource("GetICollectionTestCases")]
        public void WorksForICollection(object lhs, object rhs, bool expected)
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.EqualTo(expected));
        }

        [TestCaseSource("GetArbitraryObjectTestCases")]
        public void WorksForArbitraryObjects(object lhs, object rhs, bool expected)
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.EqualTo(expected));
        }

        [Test]
        public void ReturnsTrueForObjectsWithNoProperties()
        {
            var lhs = new Garply();
            var rhs = new Garply();

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeIDictionary()
        {
            var lhs = new Qux { Bazes = GetHashtable() };
            var rhs = new Qux { Bazes = GetHashtable() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenAValueForAKeyIsDifferentForPropertyOfTypeIDictionary()
        {
            var lhs = new Qux { Bazes = GetHashtable() };
            var rhs = new Qux { Bazes = GetHashtable() };
            rhs.Bazes["b"] = 0;

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsFalseWhenKeysAreDifferentForPropertyOfTypeIDictionary()
        {
            var lhs = new Qux { Bazes = GetHashtable() };
            var rhs = new Qux { Bazes = GetHashtable() };
            lhs.Bazes.Add("c", 3);
            rhs.Bazes.Add("d", 3);

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsFalseWhenCountIsDifferentForPropertyOfTypeIDictionary()
        {
            var lhs = new Qux { Bazes = GetHashtable() };
            var rhs = new Qux { Bazes = GetHashtable() };
            lhs.Bazes.Add("c", 3);

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeGenericIDictionaryWithValueTypeKeyAndValue()
        {
            var lhs = new Qux { Foos = GetValueTypeDictionary() };
            var rhs = new Qux { Foos = GetValueTypeDictionary() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenAValueForAKeyIsDifferentForPropertyOfTypeGenericIDictionaryWithValueTypeKeyAndValue()
        {
            var lhs = new Qux { Foos = GetValueTypeDictionary() };
            var rhs = new Qux { Foos = GetValueTypeDictionary() };
            rhs.Foos[2] = MyEnum.Foo;

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsFalseWhenKeysAreDifferentForPropertyOfTypeGenericIDictionaryWithValueTypeKeyAndValue()
        {
            var lhs = new Qux { Foos = GetValueTypeDictionary() };
            var rhs = new Qux { Foos = GetValueTypeDictionary() };
            lhs.Foos.Add(3, MyEnum.Bar);
            rhs.Foos.Add(4, MyEnum.Bar);

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsFalseWhenCountIsDifferentForPropertyOfTypeGenericIDictionaryWithValueTypeKeyAndValue()
        {
            var lhs = new Qux { Foos = GetValueTypeDictionary() };
            var rhs = new Qux { Foos = GetValueTypeDictionary() };
            lhs.Foos.Add(3, MyEnum.Bar);

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeGenericIDictionaryWithReferenceTypeKeyAndValue()
        {
            var lhs = new Qux { Bars = GetReferenceTypeDictionary() };
            var rhs = new Qux { Bars = GetReferenceTypeDictionary() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenAValueForAKeyIsDifferentForPropertyOfTypeGenericIDictionaryWithReferenceTypeKeyAndValue()
        {
            var lhs = new Qux { Bars = GetReferenceTypeDictionary() };
            var rhs = new Qux { Bars = GetReferenceTypeDictionary() };
            rhs.Bars["b"].Value = "0";

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsFalseWhenKeysAreDifferentForPropertyOfTypeGenericIDictionaryWithReferenceTypeKeyAndValue()
        {
            var lhs = new Qux { Bars = GetReferenceTypeDictionary() };
            var rhs = new Qux { Bars = GetReferenceTypeDictionary() };
            lhs.Bars.Add("c", new Bar { Value = "2" });
            lhs.Bars.Add("d", new Bar { Value = "2" });

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsFalseWhenCountIsDifferentForPropertyOfTypeGenericIDictionaryWithReferenceTypeKeyAndValue()
        {
            var lhs = new Qux { Bars = GetReferenceTypeDictionary() };
            var rhs = new Qux { Bars = GetReferenceTypeDictionary() };
            lhs.Bars.Add("c", new Bar { Value = "2" });

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeICollection()
        {
            var lhs = new Corge { Foos = GetCollection() };
            var rhs = new Corge { Foos = GetCollection() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenValuesAreDifferentForPropertyOfTypeICollection()
        {
            var lhsCollection = GetCollection();
            var rhsCollection = GetCollection();

            lhsCollection.Add(4);
            rhsCollection.Add(5);

            var lhs = new Corge { Foos = lhsCollection };
            var rhs = new Corge { Foos = rhsCollection };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeGenericICollectionOfValueType()
        {
            var lhs = new Corge { Bars = GetCollectionOfValueType() };
            var rhs = new Corge { Bars = GetCollectionOfValueType() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenKeysAreDifferentForPropertyOfTypeGenericICollectionOfValueType()
        {
            var lhsCollection = GetCollectionOfValueType();
            var rhsCollection = GetCollectionOfValueType();

            lhsCollection.Add(4);
            rhsCollection.Add(5);

            var lhs = new Corge { Bars = lhsCollection };
            var rhs = new Corge { Bars = rhsCollection };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeGenericICollectionOfReferenceType()
        {
            var lhs = new Corge { Bazes = GetCollectionOfReferenceType() };
            var rhs = new Corge { Bazes = GetCollectionOfReferenceType() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenKeysAreDifferentForPropertyOfTypeGenericICollectionOfReferenceType()
        {
            var lhsCollection = GetCollectionOfReferenceType();
            var rhsCollection = GetCollectionOfReferenceType();

            lhsCollection.Add("c");
            rhsCollection.Add("d");

            var lhs = new Corge { Bazes = lhsCollection };
            var rhs = new Corge { Bazes = rhsCollection };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeIEnumerable()
        {
            var lhs = new Grault { Foos = GetEnumerable() };
            var rhs = new Grault { Foos = GetEnumerable() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenValuesAreDifferentForPropertyOfTypeIEnumerable()
        {
            var lhsEnumerable = GetEnumerable();
            var rhsEnumerable = GetEnumerable();

            lhsEnumerable.Add(4);
            rhsEnumerable.Add(5);

            var lhs = new Grault { Foos = lhsEnumerable };
            var rhs = new Grault { Foos = rhsEnumerable };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeGenericIEnumerableOfValueType()
        {
            var lhs = new Grault { Bars = GetEnumerableOfValueType() };
            var rhs = new Grault { Bars = GetEnumerableOfValueType() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenKeysAreDifferentForPropertyOfTypeGenericIEnumerableOfValueType()
        {
            var lhsEnumerable = GetEnumerableOfValueType();
            var rhsEnumerable = GetEnumerableOfValueType();

            lhsEnumerable.Add(4);
            rhsEnumerable.Add(5);

            var lhs = new Grault { Bars = lhsEnumerable };
            var rhs = new Grault { Bars = rhsEnumerable };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void ReturnsTrueWhenAppropriateForPropertyOfTypeGenericIEnumerableOfReferenceType()
        {
            var lhs = new Grault { Bazes = GetEnumerableOfReferenceType() };
            var rhs = new Grault { Bazes = GetEnumerableOfReferenceType() };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
        }

        [Test]
        public void ReturnsFalseWhenKeysAreDifferentForPropertyOfTypeGenericIEnumerableOfReferenceType()
        {
            var lhsEnumerable = GetEnumerableOfReferenceType();
            var rhsEnumerable = GetEnumerableOfReferenceType();

            lhsEnumerable.Add("c");
            rhsEnumerable.Add("d");

            var lhs = new Grault { Bazes = lhsEnumerable };
            var rhs = new Grault { Bazes = rhsEnumerable };

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.False);
        }

        [Test]
        public void WhenTheClassImplementsIEquatableThenTheIEquatableEqualsMethodIsCalled()
        {
            var lhs = new Plugh(true);
            var rhs = new Plugh(true);

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
            Assert.That(lhs.WasEqualsCalled || rhs.WasEqualsCalled, Is.True);
        }

        [Test]
        public void WhenTheClassOverridesTheEqualsMethodThanTheOverriddenEqualsMethodIsCalled()
        {
            var lhs = new Xyzzy(true, -1);
            var rhs = new Xyzzy(true, -1);

            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.True);
            Assert.That(lhs.WasEqualsCalled || rhs.WasEqualsCalled, Is.True);
        }

        private static Hashtable GetHashtable()
        {
            return new Hashtable
            {
                { "a", 1 },
                { "b", 2 },
            };
        }

        private static ValueTypeDictionary GetValueTypeDictionary()
        {
            return new ValueTypeDictionary
            {
                { 1, MyEnum.Bar },
                { 2, MyEnum.Baz },
            };
        }

        private static ReferenceTypeDictionary GetReferenceTypeDictionary()
        {
            return new ReferenceTypeDictionary
            {
                { "a", new Bar { Value = "1" } },
                { "b", new Bar { Value = "2" } },
            };
        }

        private class ValueTypeDictionary : Dictionary<int, MyEnum>
        {
        }

        private class ReferenceTypeDictionary : Dictionary<string, Bar>
        {
        }

        private static MyCollection GetCollection()
        {
            return new MyCollection { 1, 2, 3 };
        }

        private static CollectionOfValueType GetCollectionOfValueType()
        {
            return new CollectionOfValueType { 1, 2, 3 };
        }

        private static CollectionOfReferenceType GetCollectionOfReferenceType()
        {
            return new CollectionOfReferenceType { "a", "b", "c" };
        }

        private class MyCollection : ICollection
        {
            private readonly ArrayList _list = new ArrayList();
            public void Add(object obj) { _list.Add(obj); }
            public IEnumerator GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }
            public void CopyTo(Array array, int index) { ((ICollection)_list).CopyTo(array, index); }
            public int Count { get { return _list.Count; } }
            public object SyncRoot { get { return _list.SyncRoot; } }
            public bool IsSynchronized { get { return _list.IsSynchronized; } }
        }

        private class CollectionOfValueType : ICollection<int>
        {
            private readonly List<int> _list = new List<int>();
            public IEnumerator<int> GetEnumerator() { return _list.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }
            public void Add(int item) { _list.Add(item); }
            public void Clear() { _list.Clear(); }
            public bool Contains(int item) { return _list.Contains(item); }
            public void CopyTo(int[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }
            public bool Remove(int item) { return _list.Remove(item); }
            public int Count { get { return _list.Count; } }
            public bool IsReadOnly { get { return ((ICollection<int>)_list).IsReadOnly; } }
        }

        private class CollectionOfReferenceType : ICollection<string>
        {
            private readonly List<string> _list = new List<string>();
            public IEnumerator<string> GetEnumerator() { return _list.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }
            public void Add(string item) { _list.Add(item); }
            public void Clear() { _list.Clear(); }
            public bool Contains(string item) { return _list.Contains(item); }
            public void CopyTo(string[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }
            public bool Remove(string item) { return _list.Remove(item); }
            public int Count { get { return _list.Count; } }
            public bool IsReadOnly { get { return ((ICollection<string>)_list).IsReadOnly; } }
        }

        private static MyEnumerable GetEnumerable()
        {
            return new MyEnumerable { 1, 2, 3 };
        }

        private static EnumerableOfValueType GetEnumerableOfValueType()
        {
            return new EnumerableOfValueType { 1, 2, 3 };
        }

        private static EnumerableOfReferenceType GetEnumerableOfReferenceType()
        {
            return new EnumerableOfReferenceType { "a", "b", "c" };
        }

        private class MyEnumerable : IEnumerable
        {
            private readonly ArrayList _list = new ArrayList();
            public IEnumerator GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }
            public void Add(object item) { _list.Add(item); }
        }

        private class EnumerableOfValueType : IEnumerable<int>
        {
            private readonly List<int> _list = new List<int>();
            public IEnumerator<int> GetEnumerator() { return _list.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }
            public void Add(int item) { _list.Add(item); }
        }

        private class EnumerableOfReferenceType : IEnumerable<string>
        {
            private readonly List<string> _list = new List<string>();
            public IEnumerator<string> GetEnumerator() { return _list.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }
            public void Add(string item) { _list.Add(item); }
        }

        private static IEnumerable<TestCaseData> GetIEnumerableTestCases()
        {
            yield return
                new TestCaseData(
                    new EnumerableImplementation(5),
                    new EnumerableImplementation(5),
                    true).SetName("EnumerableImplementation with equal values");

            yield return
                new TestCaseData(
                    new EnumerableImplementation(5),
                    new EnumerableImplementation(4),
                    false).SetName("EnumerableImplementation with different values");

            yield return
                new TestCaseData(
                    Enumerable.Range(1, 5),
                    Enumerable.Range(1, 5),
                    true).SetName("IEnumerable<int> with equal values");

            yield return
                new TestCaseData(
                    Enumerable.Range(1, 5),
                    Enumerable.Range(1, 4),
                    false).SetName("IEnumerable<int> with different values");

            yield return
                new TestCaseData(
                    new EnumerableOfIntImplementation(5),
                    new EnumerableOfIntImplementation(5),
                    true).SetName("EnumerableOfIntImplementation with equal values");

            yield return
                new TestCaseData(
                    new EnumerableOfIntImplementation(5),
                    new EnumerableOfIntImplementation(4),
                    false).SetName("EnumerableOfIntImplementation with different values");

            yield return
                new TestCaseData(
                    new EnumerableOfBarImplementation(5),
                    new EnumerableOfBarImplementation(5),
                    true).SetName("EnumerableOfBarImplementation with equal values");

            yield return
                new TestCaseData(
                    new EnumerableOfBarImplementation(5),
                    new EnumerableOfBarImplementation(4),
                    false).SetName("EnumerableOfBarImplementation with different values");
        }

        private class EnumerableImplementation : IEnumerable
        {
            private readonly int _itemCount;

            public EnumerableImplementation(int itemCount)
            {
                _itemCount = itemCount;
            }

            public IEnumerator GetEnumerator()
            {
                return Enumerable.Range(1, _itemCount).ToArray().GetEnumerator();
            }
        }

        private class EnumerableOfIntImplementation : IEnumerable<int>
        {
            private readonly int _itemCount;

            public EnumerableOfIntImplementation(int itemCount)
            {
                _itemCount = itemCount;
            }

            public IEnumerator<int> GetEnumerator()
            {
                return Enumerable.Range(1, _itemCount).ToList().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class EnumerableOfBarImplementation : IEnumerable<Bar>
        {
            private readonly int _itemCount;

            public EnumerableOfBarImplementation(int itemCount)
            {
                _itemCount = itemCount;
            }

            public IEnumerator<Bar> GetEnumerator()
            {
                return Enumerable.Range(1, _itemCount).Select(i => new Bar { Value = i.ToString() }).ToList().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private static IEnumerable<TestCaseData> GetICollectionTestCases()
        {
            yield return
                new TestCaseData(
                    new CollectionImplementation(5),
                    new CollectionImplementation(5),
                    true).SetName("CollectionImplementation with equal values");

            yield return
                new TestCaseData(
                    new CollectionImplementation(5),
                    new CollectionImplementation(4),
                    false).SetName("CollectionImplementation with different values");

            yield return
                new TestCaseData(
                    new CollectionOfIntImplementation(5),
                    new CollectionOfIntImplementation(5),
                    true).SetName("CollectionOfIntImplementation with equal values");

            yield return
                new TestCaseData(
                    new CollectionOfIntImplementation(5),
                    new CollectionOfIntImplementation(4),
                    false).SetName("CollectionOfIntImplementation with different values");

            yield return
                new TestCaseData(
                    new CollectionOfBarImplementation(5),
                    new CollectionOfBarImplementation(5),
                    true).SetName("CollectionOfBarImplementation with equal values");

            yield return
                new TestCaseData(
                    new CollectionOfBarImplementation(5),
                    new CollectionOfBarImplementation(4),
                    false).SetName("CollectionOfBarImplementation with different values");

            // TODO: Add test cases for containers CollectionImplementation and CollectionOfIntImplementation with items of type Bar.
        }

        private class CollectionImplementation : ICollection
        {
            private readonly ArrayList _list;
            public CollectionImplementation(int count) { _list = new ArrayList(Enumerable.Range(1, count).ToList()); }
            public IEnumerator GetEnumerator() { return _list.GetEnumerator(); }
            public void CopyTo(Array array, int index) { _list.CopyTo(array, index); }
            public int Count { get { return _list.Count; } }
            public object SyncRoot { get { return _list.SyncRoot; } }
            public bool IsSynchronized { get { return _list.IsSynchronized; } }
        }

        private class CollectionOfIntImplementation : ICollection<int>
        {
            private readonly List<int> _list;
            public CollectionOfIntImplementation(int count) { _list = new List<int>(Enumerable.Range(1, count)); }
            public IEnumerator<int> GetEnumerator() { return _list.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }
            public void Add(int item) { _list.Add(item); }
            public void Clear() { _list.Clear(); }
            public bool Contains(int item) { return _list.Contains(item); }
            public void CopyTo(int[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }
            public bool Remove(int item) { return _list.Remove(item); }
            public int Count { get { return _list.Count; } }
            public bool IsReadOnly { get { return ((ICollection<int>)_list).IsReadOnly; } }
        }

        private class CollectionOfBarImplementation : ICollection<Bar>
        {
            private readonly List<Bar> _list;
            public CollectionOfBarImplementation(int count) { _list = new List<Bar>(Enumerable.Range(1, count).Select(i => new Bar { Value = i.ToString() })); }
            public IEnumerator<Bar> GetEnumerator() { return _list.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }
            public void Add(Bar item) { _list.Add(item); }
            public void Clear() { _list.Clear(); }
            public bool Contains(Bar item) { return _list.Contains(item); }
            public void CopyTo(Bar[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }
            public bool Remove(Bar item) { return _list.Remove(item); }
            public int Count { get { return _list.Count; } }
            public bool IsReadOnly { get { return ((ICollection<Bar>)_list).IsReadOnly; } }
        }

        private static IEnumerable<TestCaseData> GetArbitraryObjectTestCases()
        {
            yield return new TestCaseData(
                GetBarInstance(),
                GetBarInstance(),
                true).SetName("Bar with same values");

            yield return new TestCaseData(
                GetBarInstance(),
                GetDifferentBarInstance(),
                false).SetName("Bar with different values");

            yield return new TestCaseData(
                GetFooInstance(),
                GetFooInstance(),
                true).SetName("Foo with same values");

            yield return new TestCaseData(
                GetFooInstance(),
                GetDifferentFooInstance(),
                false).SetName("Foo with different values");

            yield return new TestCaseData(
                GetBazInstance(),
                GetBazInstance(),
                true).SetName("Baz with same values");

            yield return new TestCaseData(
                GetBazInstance(),
                GetDifferentBazInstance(),
                false).SetName("Baz with different values");
        }

        private static Bar GetBarInstance()
        {
            return new Bar
            {
                Value = "123"
            };
        }

        private static Bar GetDifferentBarInstance()
        {
            return new Bar
            {
                Value = "789"
            };
        }

        private static Foo GetFooInstance()
        {
            return new Foo
            {
                Collection = new CollectionImplementation(5),
                CollectionOfT = new CollectionOfIntImplementation(5),
                Enumerable = new EnumerableImplementation(5),
                EnumerableOfT = new EnumerableOfIntImplementation(5),
                Int32 = 123,
                MyEnum = MyEnum.Baz,
                String = "abc"
            };
        }

        private static Foo GetDifferentFooInstance()
        {
            return new Foo
            {
                Collection = new CollectionImplementation(5),
                CollectionOfT = new CollectionOfIntImplementation(4),
                Enumerable = new EnumerableImplementation(5),
                EnumerableOfT = new EnumerableOfIntImplementation(5),
                Int32 = 123,
                MyEnum = MyEnum.Baz,
                String = "abc"
            };
        }

        private static Baz GetBazInstance()
        {
            return new Baz
            {
                Foos = new[]
                {
                    GetFooInstance(),
                    new Foo
                    {
                        Collection = new CollectionImplementation(3),
                        CollectionOfT = new CollectionOfIntImplementation(3),
                        Enumerable = new EnumerableImplementation(3),
                        EnumerableOfT = new EnumerableOfIntImplementation(3),
                        Int32 = 789,
                        MyEnum = MyEnum.Bar,
                        String = "xyz"
                    }
                },
                Bars = new Dictionary<int, Bar>
                {
                    { 1, new Bar { Value = "1" } },
                    { 2, new Bar { Value = "2" } },
                    { 3, new Bar { Value = "3" } }
                }
            };
        }

        private static Baz GetDifferentBazInstance()
        {
            return new Baz
            {
                Foos = new[]
                {
                    GetFooInstance(),
                    new Foo
                    {
                        Collection = new CollectionImplementation(3),
                        CollectionOfT = new CollectionOfIntImplementation(3),
                        Enumerable = new EnumerableImplementation(3),
                        EnumerableOfT = new EnumerableOfIntImplementation(3),
                        Int32 = 789,
                        MyEnum = MyEnum.Bar,
                        String = "xyz"
                    }
                },
                Bars = new Dictionary<int, Bar>
                {
                    { 1, new Bar { Value = "1" } },
                    { 2, new Bar { Value = "2" } },
                    { 3, new Bar { Value = "4" } }
                }
            };
        }
    }

    public class TheGetHashCodeMethod
    {
        [TestCase(1)]
        [TestCase("a")]
        [TestCase(MyEnum.Foo)]
        public void ReturnsTheValueOfTheObjectsGetHashCodeMethodForValues(object obj)
        {
            Assert.That(DeepEqualityComparer.Instance.GetHashCode(obj), Is.EqualTo(obj.GetHashCode()));
        }

        [Test]
        public void ReturnsZeroForNull()
        {
            Assert.That(DeepEqualityComparer.Instance.GetHashCode(null), Is.EqualTo(0));
        }

        [Test]
        public void ReturnsZeroForAnObjectWithNoProperties()
        {
            Assert.That(DeepEqualityComparer.Instance.GetHashCode(new Garply()), Is.EqualTo(0));
        }

        [Test]
        public void ReturnsTheAggregationOfTheHashCodeOfEachOfItsProperties()
        {
            var obj = new Waldo
            {
                Foo = "abc",
                Bar = 123,
                Baz = MyEnum.Baz
            };

            Assert.That(
                DeepEqualityComparer.Instance.GetHashCode(obj),
                Is.EqualTo(
                    AccumulateHashCode(
                        AccumulateHashCode(
                            AccumulateHashCode(0, "abc"),
                            123),
                        MyEnum.Baz)));
        }

        [Test]
        public void ReturnsTheAggregationOfTheHashCodeOfEachOfItsPropertiesWhenTheTypeHasACycle()
        {
            var obj = new Fred
            {
                Value = "abc",
                Child = new Fred
                {
                    Value = "xyz"
                }
            };

            Assert.That(
                DeepEqualityComparer.Instance.GetHashCode(obj),
                Is.EqualTo(
                    AccumulateHashCode(
                        AccumulateHashCode(0, "abc"),
                        AccumulateHashCode(
                            AccumulateHashCode(0, "xyz"),
                            null))));
        }

        [Test]
        public void WhenTheClassOverridesTheGetHashCodeMethodThanTheOverriddenGetHashCodeMethodIsCalled()
        {
            var obj = new Xyzzy(true, -1);

            Assert.That(DeepEqualityComparer.Instance.GetHashCode(obj), Is.EqualTo(-1));
            Assert.That(obj.WasGetHashCodeCalled, Is.True);
        }

        private static int AccumulateHashCode(int previousHashCode, object currentObject)
        {
            unchecked
            {
                return (previousHashCode * 397) ^ (currentObject != null ? currentObject.GetHashCode() : 0);
            }
        }
    }

    public class Foo
    {
        public int Int32 { get; set; }
        public string String { get; set; }
        public MyEnum MyEnum { get; set; }
        public IEnumerable Enumerable { get; set; }
        public IEnumerable<int> EnumerableOfT { get; set; }
        public ICollection Collection { get; set; }
        public ICollection<int> CollectionOfT { get; set; }
    }

    public class Bar
    {
        public string Value { get; set; }
    }

    public class Baz
    {
        public IEnumerable<Foo> Foos { get; set; }
        public IDictionary<int, Bar> Bars { get; set; }
    }

    public class Qux
    {
        public IDictionary<int, MyEnum> Foos { get; set; }
        public IDictionary<string, Bar> Bars { get; set; }
        public IDictionary Bazes { get; set; }
    }

    public class Corge
    {
        public ICollection Foos { get; set; }
        public ICollection<int> Bars { get; set; }
        public ICollection<string> Bazes { get; set; }
    }

    public class Grault
    {
        public IEnumerable Foos { get; set; }
        public IEnumerable<int> Bars { get; set; }
        public IEnumerable<string> Bazes { get; set; }
    }

    public class Garply
    {
    }

    public class Waldo
    {
        public string Foo { get; set; }
        public int Bar { get; set; }
        public MyEnum Baz { get; set; }
    }

    public class Fred
    {
        public string Value { get; set; }
        public Fred Child { get; set; }
    }

    public class Plugh : IEquatable<Plugh>
    {
        private readonly bool _equals;

        public Plugh(bool @equals)
        {
            _equals = @equals;
        }

        public bool Equals(Plugh other)
        {
            WasEqualsCalled = true;
            return _equals;
        }

        public bool WasEqualsCalled { get; private set; }
    }

    public class Xyzzy
    {
        private readonly bool _equals;
        private readonly int _hashCode;

        public Xyzzy(bool @equals, int hashCode)
        {
            _equals = @equals;
            _hashCode = hashCode;
        }

        public override bool Equals(object obj)
        {
            WasEqualsCalled = true;
            return _equals;
        }

        public override int GetHashCode()
        {
            WasGetHashCodeCalled = true;
            return _hashCode;
        }

        public bool WasEqualsCalled { get; private set; }
        public bool WasGetHashCodeCalled { get; private set; }
    }

    public enum MyEnum
    {
        Foo, Bar, Baz
    }
}
