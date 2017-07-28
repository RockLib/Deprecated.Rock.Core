using RockLib.Immutable;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace RockLib.DataProtection
{
    /// <summary>
    /// An implementation of <see cref="IProtectedValue"/> that stores the sensitive
    /// data as plaintext.
    /// </summary>
    public class UnprotectedValue : IProtectedValue
    {
        private readonly object _plaintextLocker = new object();
        private Semimutable<ReadOnlyCollection<byte>> _plaintext;

        /// <summary>
        /// Gets or sets the raw plaintext value.
        /// </summary>
        public byte[] Plaintext
        {
            get => _plaintext?.Value.ToArray();
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_plaintext == null) lock (_plaintextLocker) if (_plaintext == null) _plaintext = new Semimutable<ReadOnlyCollection<byte>>();
                _plaintext.Value = new ReadOnlyCollection<byte>(value);
            }
        }

        /// <summary>
        /// Gets or sets the plaintext value as a base-64 encoded string.
        /// </summary>
        public string Base64
        {
            get
            {
                var plaintext = Plaintext;
                if (plaintext == null) return null;
                return Convert.ToBase64String(plaintext);
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                Plaintext = Convert.FromBase64String(value);
            }
        }

        /// <summary>
        /// Gets or sets the plaintext value as a UTF-8 encoded string.
        /// </summary>
        public string Utf8
        {
            get
            {
                var plaintext = Plaintext;
                if (plaintext == null) return null;
                return Encoding.UTF8.GetString(plaintext);
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                Plaintext = Encoding.UTF8.GetBytes(value);
            }
        }

        /// <summary>
        /// Gets the plain text value.
        /// </summary>
        /// <returns>The plain text value.</returns>
        public byte[] GetValue()
        {
            if (_plaintext == null)
                throw new InvalidOperationException($"This instance of {nameof(UnprotectedValue)} has not been initialized. Either {nameof(Base64)}, {nameof(Utf8)}, or {nameof(Plaintext)} must be set before {nameof(GetValue)} is called.");
            return _plaintext.Value.ToArray();
        }
    }
}
