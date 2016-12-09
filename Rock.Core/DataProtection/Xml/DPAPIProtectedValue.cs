using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
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
        private readonly Lazy<ReadOnlyCollection<byte>> _encryptedData;
        private readonly Lazy<ReadOnlyCollection<byte>> _optionalEntropy;

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
            _scope = scope;
            _encryptedData = new Lazy<ReadOnlyCollection<byte>>(() =>
            {
                if (encryptedData == null) throw new ArgumentNullException("encryptedData");
                var readOnlyEncryptedData = new ReadOnlyCollection<byte>(encryptedData);
                return readOnlyEncryptedData;
            });
            _optionalEntropy = new Lazy<ReadOnlyCollection<byte>>(() =>
                optionalEntropy == null
                    ? null
                    : new ReadOnlyCollection<byte>(optionalEntropy));
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
            _scope = scope;
            _encryptedData = new Lazy<ReadOnlyCollection<byte>>(() =>
            {
                if (encryptedData == null) throw new ArgumentNullException("encryptedData");
                var data = Convert.FromBase64String(encryptedData);
                var readOnlyEncryptedData = new ReadOnlyCollection<byte>(data);
                return readOnlyEncryptedData;
            });
            _optionalEntropy = new Lazy<ReadOnlyCollection<byte>>(() =>
                optionalEntropy == null
                    ? null
                    : new ReadOnlyCollection<byte>(Convert.FromBase64String(optionalEntropy)));
        }

        /// <summary>
        /// Gets a base-64 encoded byte array containing data encrypted using the
        /// <see cref="ProtectedData.Protect"/> method.
        /// </summary>
        public IReadOnlyList<byte> EncryptedData { get { return _encryptedData.Value; } }

        /// <summary>
        /// Gets an optional additional base-64 encoded byte array that was used to encrypt
        /// the data, or null if the additional byte array was not used.
        /// </summary>
        public IReadOnlyList<byte> OptionalEntropy { get { return _optionalEntropy.Value; } }

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
            byte[] optionalEntropy;
            try
            {
                optionalEntropy = _optionalEntropy.Value == null
                    ? null
                    : _optionalEntropy.Value.ToArray();
            }
            catch (FormatException ex)
            {
                throw new DataProtectionException("The value for the 'optionalEntropy' string, when provided, must be a valid base-64 encoded string.", ex);
            }
            try
            {
                var plainText = ProtectedData.Unprotect(
                    _encryptedData.Value.ToArray(), optionalEntropy, _scope);
                return plainText;
            }
            catch (ArgumentNullException ex) { throw new DataProtectionException("No value was provided for 'encryptedData'.", ex); }
            catch (FormatException ex) { throw new DataProtectionException("The value for 'encryptedData' was not a valid base-64 encoded string.", ex); }
            catch (SecurityException ex) { throw GetException(ex); }
            catch (CryptographicException ex) { throw GetException(ex); }
            catch (Exception ex) { throw new DataProtectionException("An unexpected exception occurred.", ex); }
        }

        private DataProtectionException GetException(Exception ex)
        {
            var message = $@"Error while unprotecting DPAPI-protected user data. {
                (Scope == DataProtectionScope.CurrentUser
                    ? "The scope is set to 'CurrentUser' - are you sure that the user that is running this application is the same user that protected the user data?"
                    : "The scope is set to 'LocalMachine' - are you sure that the machine that is running this application is the same machine that was used to protect the user data?")}";

            var exception = new DataProtectionException(message, ex);
            return exception;
        }
    }
}