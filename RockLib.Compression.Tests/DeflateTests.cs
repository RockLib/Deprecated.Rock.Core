using System;
using System.IO;
using System.Text;
using Xunit;
using FluentAssertions;

namespace RockLib.Compression.Tests
{
    public class DeflateTests
    {
        [Fact]
        public void DeflateICompressorIDecompressorWorks()
        {
            var compressor = new DeflateCompressor();
            var data = Encoding.UTF8.GetBytes("test");
            byte[] compressed;
            using (var inputStream = new MemoryStream(data))
            {
                compressed = compressor.Compress(inputStream);
            }
            Console.WriteLine(compressed.Length);
            compressed.Should().NotBeNull();
            var decompressor = new DeflateDecompressor();
            byte[] decompressed;
            using (var inputStream = new MemoryStream(compressed))
            {
                decompressed = decompressor.Decompress(inputStream);
            }
            decompressed.Should().NotBeNull();
            Encoding.UTF8.GetString(decompressed).Should().Be("test");
        }

        [Fact]
        public void DeflateCompressorBytesExtesionWorks()
        {
            var compressor = new DeflateCompressor();
            var compressed = compressor.Compress(Encoding.UTF8.GetBytes("test"));
            compressed.Should().NotBeNull();
            var decompressor = new DeflateDecompressor();
            var decompressed = decompressor.Decompress(compressed);
            decompressed.Should().NotBeNull();
            Encoding.UTF8.GetString(decompressed).Should().Be("test");
        }

        [Fact]
        public void DeflateCompressorStringExtesionWorks()
        {
            var compressor = new DeflateCompressor();
            var compressed = compressor.Compress("test");
            compressed.Should().NotBeNull();
            var decompressor = new DeflateDecompressor();
            var decompressed = decompressor.Decompress(compressed);
            decompressed.Should().NotBeNull();
            Encoding.UTF8.GetString(decompressed).Should().Be("test");
        }
    }
}
