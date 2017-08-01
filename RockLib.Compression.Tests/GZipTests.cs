using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace RockLib.Compression.Tests
{
    class GZipTests
    {
        [Fact]
        public void GZipICompressorIDecompressorWorks()
        {
            var compressor = new GZipCompressor();
            var data = Encoding.UTF8.GetBytes("test");
            byte[] compressed;
            using (var inputStream = new MemoryStream(data))
            {
                compressed = compressor.Compress(inputStream);
            }
            Console.WriteLine(compressed.Length);
            compressed.Should().NotBeNull();
            var decompressor = new GZipDecompressor();
            byte[] decompressed;
            using (var inputStream = new MemoryStream(compressed))
            {
                decompressed = decompressor.Decompress(inputStream);
            }
            decompressed.Should().NotBeNull();
            Encoding.UTF8.GetString(decompressed).Should().Be("test");
        }

        [Fact]
        public void GZipCompressorBytesExtesionWorks()
        {
            var compressor = new GZipCompressor();
            var compressed = compressor.Compress(Encoding.UTF8.GetBytes("test"));
            compressed.Should().NotBeNull();
            var decompressor = new GZipDecompressor();
            var decompressed = decompressor.Decompress(compressed);
            decompressed.Should().NotBeNull();
            Encoding.UTF8.GetString(decompressed).Should().Be("test");
        }

        [Fact]
        public void GZipCompressorStringExtesionWorks()
        {
            var compressor = new GZipCompressor();
            var compressed = compressor.Compress("test");
            compressed.Should().NotBeNull();
            var decompressor = new GZipDecompressor();
            var decompressed = decompressor.Decompress(compressed);
            decompressed.Should().NotBeNull();
            Encoding.UTF8.GetString(decompressed).Should().Be("test");
        }
    }
}
