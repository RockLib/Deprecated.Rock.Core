using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rock.DataProtection;
using Rock.DataProtection.Xml;

namespace Rock.Core.UnitTests.DataProtection.Xml
{
    public class UnprotectedValueTests
    {
        [Test]
        public void CannotPassNullToByteArrayConstructor()
        {
            Assert.That(() => new UnprotectedValue((IList<byte>)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CannotPassNullToStringConstructor()
        {
            Assert.That(() => new UnprotectedValue((string)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CannotPassNonBase64EncodedStringToStringConstructor()
        {
            Assert.That(() => new UnprotectedValue("wtf"), Throws.TypeOf<FormatException>());
        }

        [Test]
        public void CanPassBase64EncodedStringToStringConstructor()
        {
            Assert.That(() => new UnprotectedValue(Convert.ToBase64String(new byte[] { 1, 2, 3 })), Throws.Nothing);
        }

        [Test]
        public void TheByteArrayPassedToTheByteArrayConstructorIsReturnedByGetValueMethod()
        {
            var data = new byte[] { 1, 2, 3 };

            IProtectedValue value = new UnprotectedValue(data);

            Assert.That(value.GetValue(), Is.EqualTo(data));
        }

        [Test]
        public void TheBase64EncodedStringPassedToTheStringConstructorIsDecodedAndReturnedByGetValueMethod()
        {
            var data = new byte[] { 1, 2, 3 };
            var stringData = Convert.ToBase64String(data);

            IProtectedValue value = new UnprotectedValue(stringData);

            Assert.That(value.GetValue(), Is.EqualTo(data));
        }
    }
}