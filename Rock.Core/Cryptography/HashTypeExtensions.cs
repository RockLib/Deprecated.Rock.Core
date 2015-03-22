using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Rock.StringFormatting;

namespace Rock.Cryptography
{
    /// <summary>
    /// HashType extensions
    /// </summary>
    public static class HashTypeExtensions
    {
        /// <summary>
        /// Converts a string to a Hash of the specified type.
        /// </summary>
        /// <param name="value">The string to be hashed.</param>
        /// <param name="hashType">Type of the hash.</param>
        /// <returns>The hash value</returns>
        public static string ToHash(this string value, HashType hashType)
        {
            return hashType.ComputeHash(value);
        }

        /// <summary>
        /// Computes the hash value for the specified byte array.
        /// </summary>
        /// <param name="hashType">The type of algorithm to use when computing the hash.</param>
        /// <param name="data">The input to compute the hash code for.</param>
        /// <returns>The computed hash code.</returns>
        public static string ToHash(this byte[] data, HashType hashType)
        {
            return hashType.ComputeHash(data);
        }

        /// <summary>
        /// Computes the hash value for the specified string.
        /// </summary>
        /// <param name="hashType">The type of algorithm to use when computing the hash.</param>
        /// <param name="value">The input to compute the hash code for.</param>
        /// <param name="encoding">
        /// The encoding to use when retrieving the binary representation of <paramref name="value"/>.
        /// If null or not supplied, then <see cref="Encoding.UTF8"/> is used instead.
        /// </param>
        /// <returns>The computed hash code.</returns>
        public static string ComputeHash(this HashType hashType, string value, Encoding encoding = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            encoding = encoding ?? Encoding.UTF8;

            return hashType.ComputeHash(encoding.GetBytes(value));
        }

        /// <summary>
        /// Computes the hash value for the specified <see cref="Stream"/> object.
        /// </summary>
        /// <param name="hashType">The type of algorithm to use when computing the hash.</param>
        /// <param name="inputStream">The input to compute the hash code for.</param>
        /// <returns>The computed hash code.</returns>
        public static string ComputeHash(this HashType hashType, Stream inputStream)
        {
            return hashType.ComputeHash(inputStream, false);
        }

        /// <summary>
        /// Computes the hash value for the specified <see cref="Stream"/> object.
        /// </summary>
        /// <param name="hashType">The type of algorithm to use when computing the hash.</param>
        /// <param name="inputStream">The input to compute the hash code for.</param>
        /// <param name="closeStream">Whether to close the stream after calculating the hash.</param>
        /// <returns>The computed hash code.</returns>
        public static string ComputeHash(this HashType hashType, Stream inputStream, bool closeStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            byte[] hash;

            using (var algorithm = GetAlgorithm(hashType))
            {
                try
                {
                    hash = algorithm.ComputeHash(inputStream);
                }
                finally
                {
                    if (closeStream)
                    {
                        inputStream.Close();
                    }
                }
            }

            return hash.ToHexString(false);
        }

        /// <summary>
        /// Computes the hash value for the specified byte array.
        /// </summary>
        /// <param name="hashType">The type of algorithm to use when computing the hash.</param>
        /// <param name="buffer">The input to compute the hash code for.</param>
        /// <returns>The computed hash code.</returns>
        public static string ComputeHash(this HashType hashType, byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            byte[] hash;

            using (var algorithm = GetAlgorithm(hashType))
            {
                hash = algorithm.ComputeHash(buffer);
            }

            return hash.ToHexString(false);
        }

        private static HashAlgorithm GetAlgorithm(HashType hashType)
        {
            switch (hashType)
            {
                case HashType.MD5:
                    return MD5.Create();
                case HashType.SHA1:
                    return SHA1.Create();
                case HashType.SHA256:
                    return SHA256.Create();
                case HashType.SHA512:
                    return SHA512.Create();
                default:
                    throw new ArgumentException("Invalid hash type", "hashType");
            }
        }
    }
}