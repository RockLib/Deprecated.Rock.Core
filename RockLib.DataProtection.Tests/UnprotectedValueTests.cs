using System;
using System.Text;
using Xunit;

namespace RockLib.DataProtection.Tests
{
    public class UnprotectedValueTests
    {
        [Fact]
        public void GetValueReturnsTheValueOfThePlaintextProperty()
        {
            byte[] plaintext = GetPlaintext();

            var unprotectedValue = new UnprotectedValue { Plaintext = plaintext };

            Assert.Equal(plaintext, unprotectedValue.GetValue());
        }

        [Fact]
        public void GetValueReturnsTheBase64DecodedValueOfTheBase64Property()
        {
            string base64 = GetBase64();

            var unprotectedValue = new UnprotectedValue { Base64 = base64 };

            Assert.Equal(Convert.FromBase64String(base64), unprotectedValue.GetValue());
        }

        [Fact]
        public void GetValueReturnsTheUtf8DecodedValueOfTheUtf8Property()
        {
            string utf8 = GetUtf8();

            var unprotectedValue = new UnprotectedValue { Utf8 = utf8 };

            Assert.Equal(Encoding.UTF8.GetBytes(utf8), unprotectedValue.GetValue());
        }

        [Fact]
        public void PlaintextIsNullWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedValue();

            Assert.Null(unprotectedValue.Plaintext);
        }

        [Fact]
        public void Base64IsNullWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedValue();

            Assert.Null(unprotectedValue.Base64);
        }

        [Fact]
        public void Utf8IsNullWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedValue();

            Assert.Null(unprotectedValue.Utf8);
        }

        [Fact]
        public void GetValueThrowsInvalidOperationExceptionWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedValue();

            Assert.Throws<InvalidOperationException>(() => unprotectedValue.GetValue());
        }

        [Fact]
        public void PassingNullToPlaintextPropertyThrowsArgumentNullException()
        {
            var unprotectedValue = new UnprotectedValue();

            Assert.Throws<ArgumentNullException>(() => unprotectedValue.Plaintext = null);
        }

        [Fact]
        public void PassingNullToBase64PropertyThrowsArgumentNullException()
        {
            var unprotectedValue = new UnprotectedValue();

            Assert.Throws<ArgumentNullException>(() => unprotectedValue.Base64 = null);
        }

        [Fact]
        public void PassingNullToUtf8PropertyThrowsArgumentNullException()
        {
            var unprotectedValue = new UnprotectedValue();

            Assert.Throws<ArgumentNullException>(() => unprotectedValue.Utf8 = null);
        }

        [Fact]
        public void VerifyPlaintextGetterAndSetterBehavior()
        {
            byte[] plaintext = GetPlaintext();

            var unprotectedValue = new UnprotectedValue { Plaintext = plaintext };

            Assert.Equal(plaintext, unprotectedValue.Plaintext);
        }

        [Fact]
        public void VerifyBase64GetterAndSetterBehavior()
        {
            string base64 = GetBase64();

            var unprotectedValue = new UnprotectedValue { Base64 = base64 };

            Assert.Equal(base64, unprotectedValue.Base64);
        }

        [Fact]
        public void VerifyUtf8GetterAndSetterBehavior()
        {
            string utf8 = GetUtf8();

            var unprotectedValue = new UnprotectedValue { Utf8 = utf8 };

            Assert.Equal(utf8, unprotectedValue.Utf8);
        }

        private static byte[] GetPlaintext() => new byte[] { 1, 2, 3, 4, 5, 6 };
        private static string GetBase64() => Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5, 6 });
        private static string GetUtf8() => "Hello, world!";
    }
}
