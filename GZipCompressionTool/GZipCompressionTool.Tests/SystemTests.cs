using System;
using System.IO;
using System.IO.Compression;
using GZipCompressionTool.Core.Models;
using log4net;
using Moq;
using Xunit;

namespace GZipCompressionTool.Tests
{
    public class SystemTests : IDisposable
    {
        private readonly string _fileToCompress = Guid.NewGuid().ToString();

        private readonly string _fileToDecompress = Guid.NewGuid().ToString();

        [Fact]
        void ProgramRun_CreateRandomFile_CompressedAndDecompressedSuccessfully()
        {
            // arrange
            var bytesToWrite = new byte[6 * 1024 * 1024];
            new Random().NextBytes(bytesToWrite);

            using (var fileStream = File.Create(_fileToCompress))
            {
                fileStream.Write(bytesToWrite, 0, bytesToWrite.Length);
            }

            Program.Logger = new Mock<ILog>().Object;

            // act
            var compressResult 
                = Program.Main(new[] { CompressionMode.Compress.ToString("G"), _fileToCompress, $"{_fileToCompress}.gz", ProgressBar.Disabled.ToString("G") });
            var decompressResult 
                = Program.Main(new[] { CompressionMode.Decompress.ToString("G"), $"{_fileToCompress}.gz", _fileToDecompress, ProgressBar.Disabled.ToString("G") });

            // assert
            Assert.Equal(0, compressResult);
            Assert.Equal(0, decompressResult);
            Assert.Equal(bytesToWrite, File.ReadAllBytes(_fileToDecompress));
        }

        public void Dispose()
        {
            if (File.Exists(_fileToCompress))
            {
                File.Delete(_fileToCompress);
            }

            if (File.Exists($"{_fileToCompress}.gz"))
            {
                File.Delete($"{_fileToCompress}.gz");
            }

            if (File.Exists(_fileToDecompress))
            {
                File.Delete(_fileToDecompress);
            }
        }
    }
}
