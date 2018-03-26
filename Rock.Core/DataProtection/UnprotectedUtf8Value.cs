using System;
using System.Text;
using RockLib.Immutable;

namespace RockLib.DataProtection
{
    public class UnprotectedUtf8Value : IProtectedValue
    {
        private readonly object _valueLocker = new object();
        private Semimutable<string> _value;

        /// <summary>
        /// Gets or sets the UTF-8 encoded string.
        /// </summary>
        public string Value
        {
            get => _value?.Value;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_value == null)
                {
                    lock (_valueLocker)
                    {
                        if (_value == null)
                        {
                            _value = new Semimutable<string>();
                        }
                    }
                }
                _value.Value = value;
            }
        }

        /// <summary>
        /// Gets the binary value.
        /// </summary>
        /// <returns>The binary value.</returns>
        public byte[] GetValue()
        {
            if (_value == null)
                throw new InvalidOperationException($"This instance of {nameof(UnprotectedUtf8Value)} has not been initialized. {nameof(Value)} must be set before {nameof(GetValue)} is called.");
            return Encoding.UTF8.GetBytes(_value.Value);
        }
    }
}
