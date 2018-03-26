using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Rock.Compression;

namespace Rock.Core.UnitTests.Compression
{
    [TestFixture]
    public class CompressionTests
    {
        [Test]
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
            Assert.IsNotNull(compressed);
            var decompressor = new GZipDecompressor();
            byte[] decompressed;
            using (var inputStream = new MemoryStream(compressed))
            {
                decompressed = decompressor.Decompress(inputStream);
            }
            Assert.IsNotNull(decompressed);
            Assert.AreEqual("test", Encoding.UTF8.GetString(decompressed));
        }

        [Test]
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
            Assert.IsNotNull(compressed);
            var decompressor = new DeflateDecompressor();
            byte[] decompressed;
            using (var inputStream = new MemoryStream(compressed))
            {
                decompressed = decompressor.Decompress(inputStream);
            }
            Assert.IsNotNull(decompressed);
            Assert.AreEqual("test", Encoding.UTF8.GetString(decompressed));
        }

        [Test]
        public void DeflateCompressorBytesExtesionWorks()
        {
            var compressor = new DeflateCompressor();
            var compressed = compressor.Compress(Encoding.UTF8.GetBytes("test"));
            Assert.IsNotNull(compressed);
            var decompressor = new DeflateDecompressor();
            var decompressed = decompressor.Decompress(compressed);
            Assert.IsNotNull(decompressed);
            Assert.AreEqual("test", Encoding.UTF8.GetString(decompressed));
        }

        [Test]
        public void GZipCompressorBytesExtesionWorks()
        {
            var compressor = new GZipCompressor();
            var compressed = compressor.Compress(Encoding.UTF8.GetBytes("test"));
            Assert.IsNotNull(compressed);
            var decompressor = new GZipDecompressor();
            var decompressed = decompressor.Decompress(compressed);
            Assert.IsNotNull(decompressed);
            Assert.AreEqual("test", Encoding.UTF8.GetString(decompressed));
        }

        [Test]
        public void DeflateCompressorStringExtesionWorks()
        {
            var compressor = new DeflateCompressor();
            var compressed = compressor.Compress("test");
            Assert.IsNotNull(compressed);
            var decompressor = new DeflateDecompressor();
            var decompressed = decompressor.Decompress(compressed);
            Assert.IsNotNull(decompressed);
            Assert.AreEqual("test", Encoding.UTF8.GetString(decompressed));
        }

        [Test]
        public void GZipCompressorStringExtesionWorks()
        {
            var compressor = new GZipCompressor();
            var compressed = compressor.Compress("test");
            Assert.IsNotNull(compressed);
            var decompressor = new GZipDecompressor();
            var decompressed = decompressor.Decompress(compressed);
            Assert.IsNotNull(decompressed);
            Assert.AreEqual("test", Encoding.UTF8.GetString(decompressed));
        }
    }
}