using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Rock.DataProtection.Xml;

namespace Rock.Core.UnitTests.DataProtection.Xml
{
    public class DPAPIProtectedValueTests
    {
        [Test]
        public void CannotPassNullToTheByteArrayConstructor()
        {
            Assert.That(() => new DPAPIProtectedValue((IList<byte>)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CannotPassNullToTheStringConstructor()
        {
            Assert.That(() => new DPAPIProtectedValue((string)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CannotPassNonBase64EncodedStringToTheStringConstructor()
        {
            Assert.That(() => new DPAPIProtectedValue("wtf"), Throws.TypeOf<FormatException>());
        }

        [Test]
        public void CanPassBase64EncodedStringToTheStringConstructor()
        {
            Assert.That(() => new DPAPIProtectedValue(Convert.ToBase64String(new byte[] { 1, 2, 3 })), Throws.Nothing);
        }

        [Test]
        public void TheEncryptedDataByteArrayPassedToTheByteArrayConstructorIsReturnedByTheEncryptedDataProperty()
        {
            var encryptedData = new byte[] { 1, 2, 3 };

            var value = new DPAPIProtectedValue(encryptedData);

            Assert.That(value.EncryptedData, Is.EqualTo(encryptedData));
        }

        [Test]
        public void TheEncryptedDataBase64EncodedStringPassedToTheStringConstructorIsDecodedAndReturnedByTheEncryptedDataProperty()
        {
            var encryptedDataBytes = new byte[] { 1, 2, 3 };
            var encryptedData = Convert.ToBase64String(encryptedDataBytes);

            var value = new DPAPIProtectedValue(encryptedData);

            Assert.That(value.EncryptedData, Is.EqualTo(encryptedDataBytes));
        }

        [Test]
        public void TheOptionalEntropyByteArrayPassedToTheByteArrayConstructorIsReturnedByTheOptionalEntropyProperty()
        {
            var encryptedData = new byte[] { 1, 2, 3 };
            var optionalEntropy = new byte[] { 4, 5, 6 };

            var value = new DPAPIProtectedValue(encryptedData, optionalEntropy);

            Assert.That(value.OptionalEntropy, Is.EqualTo(optionalEntropy));
        }

        [Test]
        public void TheOptionalEntropyBase64EncodedStringPassedToTheStringConstructorIsDecodedAndReturnedByTheOptionalEntropyProperty()
        {
            var encryptedDataBytes = new byte[] { 1, 2, 3 };
            var optionalEntropyBytes = new byte[] { 4, 5, 6 };
            var encryptedData = Convert.ToBase64String(encryptedDataBytes);
            var optionalEntropy = Convert.ToBase64String(optionalEntropyBytes);

            var value = new DPAPIProtectedValue(encryptedData, optionalEntropy);

            Assert.That(value.OptionalEntropy, Is.EqualTo(optionalEntropyBytes));
        }

        [Test]
        public void TheScopePassedToTheByteArrayConstructorIsReturnedByTheScopeProperty()
        {
            var encryptedData = new byte[] { 1, 2, 3 };

            var value = new DPAPIProtectedValue(encryptedData, scope:DataProtectionScope.LocalMachine);

            Assert.That(value.Scope, Is.EqualTo(DataProtectionScope.LocalMachine));
        }

        [Test]
        public void TheScopePassedToTheStringConstructorIsReturnedByTheScopeProperty()
        {
            var encryptedDataBytes = new byte[] { 1, 2, 3 };
            var encryptedData = Convert.ToBase64String(encryptedDataBytes);

            var value = new DPAPIProtectedValue(encryptedData, scope:DataProtectionScope.LocalMachine);

            Assert.That(value.Scope, Is.EqualTo(DataProtectionScope.LocalMachine));
        }

        [Test]
        public void CanUnprotectDPAPIProtectedValue()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var optionalEntropy = new byte[] { 4, 5, 6 };
            var scope = DataProtectionScope.LocalMachine;

            var encryptedData = ProtectedData.Protect(userData, optionalEntropy, scope);

            Assume.That(encryptedData, Is.Not.EqualTo(userData));

            var protectedValue = new DPAPIProtectedValue(encryptedData, optionalEntropy, scope);

            var unprotectedValue = protectedValue.GetValue();

            Assert.That(Encoding.UTF8.GetString(unprotectedValue), Is.EqualTo("Hello, world!"));
        }

        [Test]
        public void CanUnprotectDPAPIProtectedValueWhenNoOptionalEntropyIsProvided()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var scope = DataProtectionScope.LocalMachine;

            var encryptedData = ProtectedData.Protect(userData, null, scope);

            Assume.That(encryptedData, Is.Not.EqualTo(userData));

            var protectedValue = new DPAPIProtectedValue(encryptedData, scope:scope);

            var unprotectedValue = protectedValue.GetValue();

            Assert.That(Encoding.UTF8.GetString(unprotectedValue), Is.EqualTo("Hello, world!"));
        }

        [Test]
        public void CanUnprotectDPAPIProtectedValueWhenNoScopeIsProvided()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var optionalEntropy = new byte[] { 4, 5, 6 };

            var encryptedData = ProtectedData.Protect(userData, optionalEntropy, DataProtectionScope.CurrentUser);

            Assume.That(encryptedData, Is.Not.EqualTo(userData));

            var protectedValue = new DPAPIProtectedValue(encryptedData, optionalEntropy);

            var unprotectedValue = protectedValue.GetValue();

            Assert.That(Encoding.UTF8.GetString(unprotectedValue), Is.EqualTo("Hello, world!"));
        }

        [Test]
        public void CannotUnprotectGarbage()
        {
            var garbage = new byte[] { 1, 2, 3, 4, 5, 6 };

            var protectedValue = new DPAPIProtectedValue(garbage);

            Assert.That(() => protectedValue.GetValue(), Throws.TypeOf<CryptographicException>());
        }
    }
}