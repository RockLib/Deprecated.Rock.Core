using System.IO;

#if ROCKLIB
namespace RockLib.Compression
#else
namespace Rock.Compression
#endif
{
    /// <summary>
    /// Defines an interface for decompression.
    /// </summary>
    public interface IDecompressor
    {
        /// <summary>
        /// Decompress the contents of the stream into a byte array.
        /// </summary>
        /// <param name="inputStream">The stream to decompress.</param>
        /// <returns>The decompressed byte array.</returns>
        byte[] Decompress(Stream inputStream);
    }
}