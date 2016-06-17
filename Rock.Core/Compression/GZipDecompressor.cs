using System;
using System.IO;
using System.IO.Compression;

namespace Rock.Compression
{
    /// <summary>
    /// An implementation of <see cref="IDecompressor"/> that uses GZip.
    /// </summary>
	public class GZipDecompressor : IDecompressor
	{
        /// <summary>
        /// Decompress the contents of the stream into a byte array.
        /// </summary>
        /// <param name="inputStream">The stream to decompress.</param>
        /// <returns>The decompressed byte array.</returns>
        public byte[] Decompress(Stream inputStream)
		{
			if (inputStream == null) throw new ArgumentNullException("inputStream");
			using (var outputStream = new MemoryStream())
			{
				using (var gzStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
				{
					gzStream.CopyTo(outputStream);
				}

				return outputStream.ToArray();
			}
		}
	}
}