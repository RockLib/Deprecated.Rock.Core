using System;
using System.Text;
using Xunit;

namespace RockLib.DataProtection.Tests
{
    public class UnprotectedUtf8ValueTests
    {

        [Fact]
        public void GetValueReturnsTheUtf8DecodedValueOfTheValueProperty()
        {
            string utf8 = GetUtf8();

            var unprotectedValue = new UnprotectedUtf8Value { Value = utf8 };

            Assert.Equal(Encoding.UTF8.GetBytes(utf8), unprotectedValue.GetValue());
        }

        [Fact]
        public void ValueIsNullWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedUtf8Value();

            Assert.Null(unprotectedValue.Value);
        }

        [Fact]
        public void GetValueThrowsInvalidOperationExceptionWhenUninitialized()
        {
            var unprotectedValue = new UnprotectedUtf8Value();

            Assert.Throws<InvalidOperationException>(() => unprotectedValue.GetValue());
        }

        [Fact]
        public void PassingNullToValuePropertyThrowsArgumentNullException()
        {
            var unprotectedValue = new UnprotectedUtf8Value();

            Assert.Throws<ArgumentNullException>(() => unprotectedValue.Value = null);
        }

        [Fact]
        public void VerifyValueGetterAndSetterBehavior()
        {
            string utf8 = GetUtf8();

            var unprotectedValue = new UnprotectedUtf8Value() { Value = utf8 };

            Assert.Equal(utf8, unprotectedValue.Value);
        }

        private static string GetUtf8() => "Hello, world!";
    }
}
