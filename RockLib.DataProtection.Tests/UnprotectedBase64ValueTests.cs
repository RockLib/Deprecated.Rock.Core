using System;
using Xunit;

namespace RockLib.DataProtection.Tests
{
    public class UnprotectedBase64ValueTests
    {
        [Fact]
        public void GetValueReturnsTheBase64DecodedValueOfTheValueProperty()
        {
            string base64 = GetBase64();

            var unprotectedValue = new UnprotectedBase64Value { Value = base64 };

            Assert.Equal(Convert.FromBase64String(base64), unprotectedValue.GetValue());
        }

        [Fact]
        public void ValueIsNullWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedBase64Value();

            Assert.Null(unprotectedValue.Value);
        }

        [Fact]
        public void GetValueThrowsInvalidOperationExceptionWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedBase64Value();

            Assert.Throws<InvalidOperationException>(() => unprotectedValue.GetValue());
        }

        [Fact]
        public void PassingNullToValuePropertyThrowsArgumentNullException()
        {
            var unprotectedValue = new UnprotectedBase64Value();

            Assert.Throws<ArgumentNullException>(() => unprotectedValue.Value = null);
        }

        [Fact]
        public void VerifyValueGetterAndSetterBehavior()
        {
            string base64 = GetBase64();

            var unprotectedValue = new UnprotectedBase64Value() { Value = base64 };

            Assert.Equal(base64, unprotectedValue.Value);
        }

        private static string GetBase64() => Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5, 6 });
    }
}
