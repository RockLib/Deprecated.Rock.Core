using System;
using System.IO;
using System.IO.Compression;

#if ROCKLIB
namespace RockLib.Compression
#else
namespace Rock.Compression
#endif
{
    /// <summary>
    /// An implementation of <see cref="ICompressor"/> that uses GZip.
    /// </summary>
    public class GZipCompressor : ICompressor
    {
        /// <summary>
        /// Compress the contents of the stream into a byte array.
        /// </summary>
        /// <param name="inputStream">The stream to compress.</param>
        /// <returns>The compressed byte array.</returns>
        public byte[] Compress(Stream inputStream)
        {
            if (inputStream == null) throw new ArgumentNullException("inputStream");
            using (var outputStream = new MemoryStream())
            {
                using (var gzStream = new GZipStream(outputStream, CompressionMode.Compress, true))
                {
                    inputStream.CopyTo(gzStream);
                }

                return outputStream.ToArray();
            }
        }
    }
}