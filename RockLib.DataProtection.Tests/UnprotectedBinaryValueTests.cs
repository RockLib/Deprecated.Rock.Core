using System;
using Xunit;

namespace RockLib.DataProtection.Tests
{
    public class UnprotectedBinaryValueTests
    {
        [Fact]
        public void GetValueReturnsTheValueOfTheValueProperty()
        {
            byte[] plaintext = GetPlaintext();

            var unprotectedValue = new UnprotectedBinaryValue { Value = plaintext };

            Assert.Equal(plaintext, unprotectedValue.GetValue());
        }

        [Fact]
        public void ValueIsNullWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedBinaryValue();

            Assert.Null(unprotectedValue.Value);
        }

        [Fact]
        public void GetValueThrowsInvalidOperationExceptionWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedBinaryValue();

            Assert.Throws<InvalidOperationException>(() => unprotectedValue.GetValue());
        }

        [Fact]
        public void PassingNullToValuePropertyThrowsArgumentNullException()
        {
            var unprotectedValue = new UnprotectedBinaryValue();

            Assert.Throws<ArgumentNullException>(() => unprotectedValue.Value = null);
        }

        [Fact]
        public void VerifyValueGetterAndSetterBehavior()
        {
            byte[] plaintext = GetPlaintext();

            var unprotectedValue = new UnprotectedBinaryValue() { Value = plaintext };

            Assert.Equal(plaintext, unprotectedValue.Value);
        }

        private static byte[] GetPlaintext() => new byte[] { 1, 2, 3, 4, 5, 6 };
    }
}
