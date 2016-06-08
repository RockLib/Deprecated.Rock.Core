using System.IO;
using System.Text;

namespace Rock.Compression
{
	/// <summary>
	/// Extensions for ICompressor and IDecompressor
	/// </summary>
	public static class CompressionExtensions
	{
		/// <summary>
		/// Compress a string with optional encoding
		/// </summary>
		/// <param name="compressor">Compressor to extend</param>
		/// <param name="text">The text to compress</param>
		/// <param name="encoding">The optional encoding to use.</param>
		/// <returns></returns>
		public static byte[] Compress(this ICompressor compressor, string text, Encoding encoding = null)
		{
			if (encoding == null) encoding = Encoding.UTF8;
			using (var outputStream = new MemoryStream(encoding.GetBytes(text)))
			{
				return compressor.Compress(outputStream);
			}
		}

		/// <summary>
		/// Compress raw bytes
		/// </summary>
		/// <param name="compressor">Compressor to extend</param>
		/// <param name="data">The data to compress</param>
		/// <returns></returns>
		public static byte[] Compress(this ICompressor compressor, byte[] data)
		{
			using (var outputStream = new MemoryStream(data))
			{
				return compressor.Compress(outputStream);
			}
		}

		/// <summary>
		/// Decompress compressed bytes
		/// </summary>
		/// <param name="decompressor">Compressor to extend</param>
		/// <param name="data">The data to decompress</param>
		/// <returns></returns>
		public static byte[] Decompress(this IDecompressor decompressor, byte[] data)
		{
			using (var outputStream = new MemoryStream(data))
			{
				return decompressor.Decompress(outputStream);
			}
		}
	}
}