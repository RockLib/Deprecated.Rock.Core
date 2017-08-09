using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RockLib.Immutable;

namespace RockLib.DataProtection
{
    public class UnprotectedBinaryValue : IProtectedValue
    {
        private readonly object _valueLocker = new object();
        private Semimutable<ReadOnlyCollection<byte>> _value;

        /// <summary>
        /// Gets or sets the UTF-8 encoded string.
        /// </summary>
        public byte[] Value
        {
            get => _value?.Value.ToArray();
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_value == null)
                {
                    lock (_valueLocker)
                    {
                        if (_value == null)
                        {
                            _value = new Semimutable<ReadOnlyCollection<byte>>();
                        }
                    }
                }
                _value.Value = new ReadOnlyCollection<byte>(value);
            }

        }

        /// <summary>
        /// Gets the binary value.
        /// </summary>
        /// <returns>The binary value.</returns>
        public byte[] GetValue()
        {
            if (_value == null)
                throw new InvalidOperationException($"This instance of {nameof(UnprotectedBinaryValue)} has not been initialized. {nameof(Value)} must be set before {nameof(GetValue)} is called.");
            return _value.Value.ToArray();
        }
    }
}
