using System;
using System.IO;
using System.IO.Compression;

namespace Rock.Compression
{
    /// <summary>
    /// An implementation of <see cref="ICompressor"/> that uses a deflate stream.
    /// </summary>
	public class DeflateCompressor : ICompressor
    {
        /// <summary>
        /// Compress the contents of the stream into a byte array.
        /// </summary>
        /// <param name="inputStream">The stream to compress.</param>
        /// <returns>The compressed byte array.</returns>
        public byte[] Compress(Stream inputStream)
        {
            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
            using (var outputStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(outputStream, CompressionMode.Compress, true))
                {
                    inputStream.CopyTo(deflateStream);
                }

                return outputStream.ToArray();
            }
        }
    }
}