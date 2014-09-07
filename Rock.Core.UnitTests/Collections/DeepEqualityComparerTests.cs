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

        [Test]
        public void ReturnsFalseWhenTheTwoObjectsHaveDifferentTypes()
        {
            Assert.That(DeepEqualityComparer.Instance.Equals("abc", 123), Is.False);
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
                return Enumerable.Range(1, _itemCount).Select(i => new Bar { Value = i }).ToList().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [TestCaseSource("GetICollectionTestCases")]
        public void WorksForICollection(object lhs, object rhs, bool expected)
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.EqualTo(expected));
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
            public CollectionOfBarImplementation(int count) { _list = new List<Bar>(Enumerable.Range(1, count).Select(i => new Bar { Value = i })); }
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

        [TestCaseSource("GetArbitraryObjectTestCases")]
        public void WorksForArbitraryObjects(object lhs, object rhs, bool expected)
        {
            Assert.That(DeepEqualityComparer.Instance.Equals(lhs, rhs), Is.EqualTo(expected));
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
                Value = 123
            };
        }

        private static Bar GetDifferentBarInstance()
        {
            return new Bar
            {
                Value = 789
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
                    { 1, new Bar { Value = 1 } },
                    { 2, new Bar { Value = 2 } },
                    { 3, new Bar { Value = 3 } }
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
                    { 1, new Bar { Value = 1 } },
                    { 2, new Bar { Value = 2 } },
                    { 3, new Bar { Value = 4 } }
                }
            };
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
            public int Value { get; set; }
        }

        public class Baz
        {
            public IEnumerable<Foo> Foos { get; set; }
            public IDictionary<int, Bar> Bars { get; set; }
        }

        public enum MyEnum
        {
            Foo, Bar, Baz
        }
    }
}
