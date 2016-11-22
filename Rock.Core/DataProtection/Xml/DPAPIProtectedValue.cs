using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;

namespace Rock.DataProtection.Xml
{
    /// <summary>
    /// An implementation of <see cref="IProtectedValue"/> that stores the sensitive
    /// data using DPAPI.
    /// </summary>
    public class DPAPIProtectedValue : IProtectedValue
    {
        private readonly DataProtectionScope _scope;
        private readonly ReadOnlyCollection<byte> _encryptedData;
        private readonly ReadOnlyCollection<byte> _optionalEntropy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DPAPIProtectedValue"/> class.
        /// </summary>
        /// <param name="encryptedData">
        /// A byte array containing data encrypted using the
        /// <see cref="ProtectedData.Protect"/> method.
        /// </param>
        /// <param name="optionalEntropy">
        /// An optional additional byte array that was used to encrypt the data, or
        /// null if the additional byte array was not used.
        /// </param>
        /// <param name="scope">
        /// One of the enumeration values that specifies the scope of data protection
        /// that was used to encrypt the data.
        /// </param>
        public DPAPIProtectedValue(IList<byte> encryptedData, IList<byte> optionalEntropy = null,
            DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (encryptedData == null) throw new ArgumentNullException("encryptedData");
            _scope = scope;
            _encryptedData = new ReadOnlyCollection<byte>(encryptedData);
            _optionalEntropy = optionalEntropy == null ? null : new ReadOnlyCollection<byte>(optionalEntropy);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DPAPIProtectedValue"/> class.
        /// </summary>
        /// <param name="encryptedData">
        /// A base-64 encoded byte array containing data encrypted using the
        /// <see cref="ProtectedData.Protect"/> method.
        /// </param>
        /// <param name="optionalEntropy">
        /// An optional additional base-64 encoded byte array that was used to encrypt
        /// the data, or null if the additional byte array was not used.
        /// </param>
        /// <param name="scope">
        /// One of the enumeration values that specifies the scope of data protection
        /// that was used to encrypt the data.
        /// </param>
        public DPAPIProtectedValue(string encryptedData, string optionalEntropy = null,
            DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (encryptedData == null) throw new ArgumentNullException("encryptedData");
            _scope = scope;
            _encryptedData = new ReadOnlyCollection<byte>(Convert.FromBase64String(encryptedData));
            _optionalEntropy = optionalEntropy == null ? null : new ReadOnlyCollection<byte>(Convert.FromBase64String(optionalEntropy));
        }

        /// <summary>
        /// Gets a base-64 encoded byte array containing data encrypted using the
        /// <see cref="ProtectedData.Protect"/> method.
        /// </summary>
        public IReadOnlyList<byte> EncryptedData { get { return _encryptedData; } }

        /// <summary>
        /// Gets an optional additional base-64 encoded byte array that was used to encrypt
        /// the data, or null if the additional byte array was not used.
        /// </summary>
        public IReadOnlyList<byte> OptionalEntropy { get { return _optionalEntropy; } }

        /// <summary>
        /// Gets one of the enumeration values that specifies the scope of data protection
        /// that was used to encrypt the data.
        /// </summary>
        public DataProtectionScope Scope { get { return _scope; } }

        /// <summary>
        /// Gets the plain text value.
        /// </summary>
        /// <returns>The plain text value.</returns>
        public byte[] GetValue()
        {
            var optionalEntropy = _optionalEntropy == null
                ? null
                : _optionalEntropy.ToArray();

            var plainText = ProtectedData.Unprotect(
                _encryptedData.ToArray(), optionalEntropy, _scope);

            return plainText;
        } 
    }
}