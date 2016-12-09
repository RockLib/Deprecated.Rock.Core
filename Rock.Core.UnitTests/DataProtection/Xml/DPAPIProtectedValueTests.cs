using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Rock.DataProtection.Xml;
using Rock.DataProtection;
using System.Threading;

namespace Rock.Core.UnitTests.DataProtection.Xml
{
    public class DPAPIProtectedValueTests
    {
        private static ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

        [Test]
        public void PassingNullToTheByteArrayConstructorDoesNotThrow()
        {
            Assert.That(() => new DPAPIProtectedValue((IList<byte>)null), Throws.Nothing);
        }

        [Test]
        public void PassingNullToTheByteArrayConstructorCausesGetValueToThrow()
        {
            var protectedValue = new DPAPIProtectedValue((IList<byte>)null);
            Assert.That(() => protectedValue.GetValue(),
                Throws.InstanceOf<DataProtectionException>()
                    .With.InnerException.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void PassingNullToTheStringConstructorDoesNotThrow()
        {
            Assert.That(() => new DPAPIProtectedValue((string)null), Throws.Nothing);
        }

        [Test]
        public void PassingNullToTheStringConstructorCausesGetValueToThrow()
        {
            var protectedValue = new DPAPIProtectedValue((string)null);
            Assert.That(() => protectedValue.GetValue(),
                Throws.InstanceOf<DataProtectionException>()
                    .With.InnerException.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void PassingNonBase64EncodedStringToTheStringConstructorDoesNotThrow()
        {
            Assert.That(() => new DPAPIProtectedValue("wtf"), Throws.Nothing);
        }

        [Test]
        public void PassingNonBase64EncodedStringToTheStringConstructorCausesGetValueToThrow()
        {
            var protectedValue = new DPAPIProtectedValue("wtf");
            Assert.That(() => protectedValue.GetValue(),
                Throws.InstanceOf<DataProtectionException>()
                    .With.InnerException.InstanceOf<FormatException>());
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
        public void PassingInvalidBase64EncodedOptionalEntropyToTheStringConstructorDoesNotThrow()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var validOptionalEntropy = new byte[] { 4, 5, 6 };
            var scope = DataProtectionScope.LocalMachine;
            var encryptedData = Convert.ToBase64String(
                ProtectedData.Protect(userData, validOptionalEntropy, scope));

            var invalidOptionalEntropy = "omg!wtf!bbq!";

            Assert.That(() => new DPAPIProtectedValue(encryptedData, invalidOptionalEntropy), Throws.Nothing);
        }

        [Test]
        public void PassingInvalidBase64EncodedOptionalEntropyToTheStringConstructorCausesGetValueToThrow()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var validOptionalEntropy = new byte[] { 4, 5, 6 };
            var scope = DataProtectionScope.LocalMachine;
            var encryptedData = Convert.ToBase64String(
                ProtectedData.Protect(userData, validOptionalEntropy, scope));

            var invalidOptionalEntropy = "omg!wtf!bbq!";

            var protectedValue = new DPAPIProtectedValue(encryptedData, invalidOptionalEntropy);

            Assert.That(() => protectedValue.GetValue(),
                Throws.TypeOf<DataProtectionException>()
                    .With.InnerException.TypeOf<FormatException>());
        }

        [Test]
        public void PassingBadEncryptedDataToTheStringConstructorDoesNotThrow()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var optionalEntropy = new byte[] { 4, 5, 6 };
            var scope = DataProtectionScope.LocalMachine;

            var encryptedData = ProtectedData.Protect(userData, optionalEntropy, scope);
            encryptedData[0] ^= 0xFF;

            Assert.That(() => new DPAPIProtectedValue(Convert.ToBase64String(encryptedData), optionalEntropy == null ? null : Convert.ToBase64String(optionalEntropy)), Throws.Nothing);
        }

        [Test]
        public void PassingBadEncryptedDataToTheByteArrayConstructorDoesNotThrow()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var optionalEntropy = new byte[] { 4, 5, 6 };
            var scope = DataProtectionScope.LocalMachine;

            var encryptedData = ProtectedData.Protect(userData, optionalEntropy, scope);
            encryptedData[0] ^= 0xFF;

            Assert.That(() => new DPAPIProtectedValue(encryptedData, optionalEntropy), Throws.Nothing);
        }

        [Test]
        public void PassingBadEncryptedDataToTheStringConstructorCausesGetValueToThrow()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var optionalEntropy = new byte[] { 4, 5, 6 };
            var scope = DataProtectionScope.LocalMachine;

            var encryptedData = ProtectedData.Protect(userData, optionalEntropy, scope);
            encryptedData[0] ^= 0xFF;

            var protectedValue = new DPAPIProtectedValue(encryptedData, optionalEntropy);

            Assert.That(() => protectedValue.GetValue(),
                Throws.TypeOf<DataProtectionException>()
                    .With.InnerException.TypeOf<CryptographicException>());
        }

        [Test]
        public void PassingBadEncryptedDataToTheByteArrayConstructorCausesGetValueToThrow()
        {
            var userData = Encoding.UTF8.GetBytes("Hello, world!");
            var optionalEntropy = new byte[] { 4, 5, 6 };
            var scope = DataProtectionScope.LocalMachine;

            var encryptedData = ProtectedData.Protect(userData, optionalEntropy, scope);
            encryptedData[0] ^= 0xFF;

            var protectedValue = new DPAPIProtectedValue(Convert.ToBase64String(encryptedData), optionalEntropy == null ? null : Convert.ToBase64String(optionalEntropy));

            Assert.That(() => protectedValue.GetValue(),
                Throws.TypeOf<DataProtectionException>()
                    .With.InnerException.TypeOf<CryptographicException>());
        }
    }
}