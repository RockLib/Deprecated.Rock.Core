using System.Globalization;
using System.Text;
using NUnit.Framework;
using Rock.IO;

// ReSharper disable once CheckNamespace


namespace EncodedStringWriterTests
{
    public class TheFirstConstructor
    {
        [Test]
        public void CausesTheEncodingPropertyToReturnTheEncodingParameter()
        {
            var encoding = Encoding.BigEndianUnicode;

            var writer = new EncodedStringWriter(encoding);

            Assert.That(writer.Encoding, Is.SameAs(encoding));
        }
    }

    public class TheSecondConstructor
    {
        [Test]
        public void CausesTheEncodingPropertyToReturnTheEncodingParameter()
        {
            var encoding = Encoding.BigEndianUnicode;

            var writer = new EncodedStringWriter(new StringBuilder(), encoding);

            Assert.That(writer.Encoding, Is.SameAs(encoding));
        }
    }

    public class TheThirdConstructor
    {
        [Test]
        public void CausesTheEncodingPropertyToReturnTheEncodingParameter()
        {
            var encoding = Encoding.BigEndianUnicode;

            var writer = new EncodedStringWriter(CultureInfo.CurrentCulture, encoding);

            Assert.That(writer.Encoding, Is.SameAs(encoding));
        }
    }

    public class TheFourthConstructor
    {
        [Test]
        public void CausesTheEncodingPropertyToReturnTheEncodingParameter()
        {
            var encoding = Encoding.BigEndianUnicode;

            var writer = new EncodedStringWriter(new StringBuilder(), CultureInfo.CurrentCulture, encoding);

            Assert.That(writer.Encoding, Is.SameAs(encoding));
        }
    }
}