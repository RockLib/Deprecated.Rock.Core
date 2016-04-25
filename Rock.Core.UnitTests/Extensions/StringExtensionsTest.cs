using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rock.Extensions;
using HashType = Rock.Cryptography.HashType;

namespace Rock.Core.UnitTests.Extensions
{
    /// <summary>
	/// This is a test class for StringExtensionsTest and is intended
    /// to contain all StringExtensions Unit Tests
    /// </summary>
    [TestFixture]
    public class StringExtensionsTest
    {
        [Test]
        public void StringExtension_IsNotNullOrEmpty_Should_Pass()
        {
            string x = "Blah";
            Assert.IsTrue(x.IsNotNullOrEmpty());
        }

		[Test]
		public void StringExtension_IsNullOrEmpty_Should_Pass()
		{
			string x = "Blah";
			Assert.IsFalse(x.IsNullOrEmpty());
		}

		[Test]
		public void StringExtension_IsNotNullOrWhiteSpace_Should_Pass()
		{
			string x = "Blah";
			Assert.IsTrue(x.IsNotNullOrWhiteSpace());
		}

		[Test]
		public void StringExtension_IsNullOrWhiteSpace_Should_Pass()
		{
			string x = "Blah";
			Assert.IsFalse(x.IsNullOrWhiteSpace());
		}

		[Test]
		public void NullString_CheckIsNotNullAndEquals_ReturnsFalse()
		{
			string foo = null;
			Assert.IsFalse(foo.IsNotNullAndEquals("foo"));
		}

		[Test]
		public void NonNullStringDifferentFromComparison_CheckIsNotNullAndEquals_ReturnsFalse()
		{
			string foo = "foo";
			Assert.IsFalse(foo.IsNotNullAndEquals("bar"));
		}

		[Test]
		public void NonNullStringSameAsComparison_CheckIsNotNullAndEquals_ReturnsTrue()
		{
			string foo = "foo";
			Assert.IsTrue(foo.IsNotNullAndEquals("foo"));
		}

	    public class JsonTestClass
	    {
		    public string key1;
			public string key2;
		}

		[Test]
        public void String_Extension_Can_Deserialize_Json()
        {
            string json = @"{""key1"":""value1"",""key2"":""value2""}";

            var value = json.FromJson<JsonTestClass>();
			Assert.AreEqual(value.key1, "value1");
			Assert.AreEqual(value.key2, "value2");
        }

	    // ReSharper disable once ClassNeverInstantiated.Global
	    // ReSharper disable once MemberCanBePrivate.Global
		public class Xml
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Global
			public string Element { get; set; }
		}

		[TestCase("<Xml><Element>value</Element></Xml>", "value")]
		public void String_Extension_Can_Deserialize_Xml(string xml, string elementValue)
		{
			var o = xml.FromXml<Xml>();
			Assert.IsNotNull(o);
			Assert.AreEqual(elementValue, o.Element);
		}

        [Test]
        public void StringExtensions_ToHash_Returns_Correct_Hash()
        {
            var hash = "Hello, world!".ToHash(HashType.MD5);

            Assert.AreEqual("6cd3556deb0da54bca060b4c39479839", hash);
        }

		[Test]
		public void TruncateSucceeds()
		{
			Assert.AreEqual("truncate", "truncated".Truncate(8));
		}

		[Test]
		public void TruncateWithLargerMaxLengthSucceeds()
		{
			Assert.AreEqual("truncate", "truncate".Truncate(9));
		}

		[Test]
		public void TruncateWithNullReturnsNull()
		{
			Assert.IsNull(((string)null).Truncate(1));
		}

		[Test]
		public void ContainsWithNullReturnsFalse()
		{
			Assert.IsFalse(((string)null).Contains("text", StringComparison.CurrentCulture));
		}

		[Test]
		public void ContainsWithNotContainsReturnsFalse()
		{
			Assert.IsFalse("test".Contains("text", StringComparison.CurrentCulture));
		}

		[Test]
		public void ContainsWithExactReturnsTrue()
		{
			Assert.IsTrue("text".Contains("text", StringComparison.CurrentCulture));
		}

		[Test]
		public void ContainsWithContainsReturnsTrue()
		{
			Assert.IsTrue("the text is English".Contains("text", StringComparison.CurrentCulture));
		}

		[Test]
		public void ContainsWithExactCaseInsensitiveReturnsTrue()
		{
			Assert.IsTrue("text".Contains("text", StringComparison.OrdinalIgnoreCase));
		}

		[Test]
		public void ContainsWithContainsCaseInsensitiveReturnsTrue()
		{
			Assert.IsTrue("the text is English".Contains("text", StringComparison.OrdinalIgnoreCase));
		}

		[Test]
	    public void ContainsWithInvalidStringComparisionThrowsException()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => "X".Contains("x", (StringComparison) int.MaxValue));
		}

		[Test]
	    public void ContainsWithNullSearchTextThrowsException()
	    {
		    Assert.Throws<ArgumentNullException>(() => "x".Contains(null, StringComparison.CurrentCulture));
	    }

	    [Test]
		public void FirstWordWithSpaceSucceeds()
		{
			var actual = "first second".FirstWord();
			Assert.AreEqual("first", actual);
		}

		[Test]
		public void FirstWordWithoutSpaceSucceeds()
		{
			var actual = "first".FirstWord();
			Assert.AreEqual("first", actual);
		}

		[Test]
		public void FirstWordWithNullReturnsNull()
		{
			Assert.IsNull(((string)null).FirstWord());
		}

		[Test]
		public void LastWordWithSpaceSucceeds()
		{
			Assert.AreEqual("Second", "First Second".LastWord());
		}

		[Test]
		public void LastWordWithoutSpaceSucceeds()
		{
			Assert.AreEqual("oneword", "oneword".LastWord());
		}

		[Test]
		public void LastWordWithNullReturnsNull()
		{
			Assert.IsNull(((string)null).LastWord());
		}

		[Test]
		public void InitialsWithSpaceSucceeds()
		{
			var actual = "first second".Initials();
			Assert.AreEqual("fs", actual);
		}

		[Test]
		public void InitialsWithoutSpaceSucceeds()
		{
			var actual = "first".Initials();
			Assert.AreEqual("f", actual);
		}

		[Test]
		public void InitialsWithNullReturnsNull()
		{
			var actual = ((string)null).Initials();
			Assert.IsNull(actual);
		}

		[Test]
		public void ToFirstLastWithOneWordSucceeds()
		{
			Assert.AreEqual("Tom", "Tom".ToLastFirst());
		}

		[Test]
		public void ToFirstLastWithTwoWordSucceeds()
		{
			Assert.AreEqual("Fleming, Tom", "Tom Fleming".ToLastFirst());
		}

		[Test]
		public void ToFirstLastWithThreeWordSucceeds()
		{
			Assert.AreEqual("Fleming, Tom", "Tom Lee Fleming".ToLastFirst());
		}

		[Test]
		public void ToFirstLastWithNullSucceeds()
		{
			Assert.IsNull(((string)null).ToLastFirst());
		}
	}
}
