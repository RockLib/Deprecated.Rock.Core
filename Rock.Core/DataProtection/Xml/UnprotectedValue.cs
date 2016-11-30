using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Rock.DataProtection.Xml
{
    /// <summary>
    /// An implementation of <see cref="IProtectedValue"/> that stores the sensitive
    /// data as plain text.
    /// </summary>
    public class UnprotectedValue : IProtectedValue
    {
        private readonly ReadOnlyCollection<byte> _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprotectedValue"/> class.
        /// </summary>
        /// <param name="value">The plain text byte array value.</param>
        public UnprotectedValue(IList<byte> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            _value = new ReadOnlyCollection<byte>(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprotectedValue"/> class.
        /// </summary>
        /// <param name="value">The base-64 encoded plain text byte array value.</param>
        public UnprotectedValue(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            _value = new ReadOnlyCollection<byte>(Convert.FromBase64String(value));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprotectedValue"/> class.
        /// </summary>
        /// <param name="text">The string plain text value.</param>
        /// <param name="encoding">
        /// The encoding to be used to convert <paramref name="text"/> to a <see cref="byte[]"/>.
        /// If null, or not provided, <see cref="Encoding.UTF8"/> is used.
        /// </param>
        public UnprotectedValue(string text, Encoding encoding = null)
        {
            if (text == null) throw new ArgumentNullException("text");
            _value = new ReadOnlyCollection<byte>((encoding ?? Encoding.UTF8).GetBytes(text));
        }
        
        /// <summary>
        /// Gets the plain text value.
        /// </summary>
        public IReadOnlyList<byte> Value { get { return _value; } }

        /// <summary>
        /// Gets the plain text value.
        /// </summary>
        /// <returns>The plain text value.</returns>
        public byte[] GetValue()
        {
            return _value.ToArray();
        } 
    }
}