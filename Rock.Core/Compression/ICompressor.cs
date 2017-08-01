using System.IO;

#if ROCKLIB
namespace RockLib.Compression
#else
namespace Rock.Compression
#endif
{
    /// <summary>
    /// Defines an interface for compression.
    /// </summary>
	public interface ICompressor
	{
        /// <summary>
        /// Compress the contents of the stream into a byte array.
        /// </summary>
        /// <param name="inputStream">The stream to compress.</param>
        /// <returns>The compressed byte array.</returns>
		byte[] Compress(Stream inputStream);
	}
}