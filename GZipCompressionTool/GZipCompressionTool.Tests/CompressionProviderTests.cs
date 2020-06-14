using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using GZipCompressionTool.Core;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using Xunit;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Tests
{
    public class CompressionProviderTests
    {
        private const int ChunkSize = 5;

        [Fact]
        public void Execute_RandomBytesInput_BytesProperlyWritten()
        {
            // arrange
            var inputByteArray = new byte[12];
            new Random().NextBytes(inputByteArray);
            var inputMemoryStream = new MemoryStream(inputByteArray);
            var outputByteArray = new byte[100];
            var compressionSynchronizationContext = new CompressionSynchronizationContext(1);
            var compressionOptions = new CompressionOptions { CompressionMode = CompressionMode.Compress, ReadBufferSize = ChunkSize };

            // act
            using (var outputStream = new MemoryStream(outputByteArray))
            {
                GetCompressionProvider(compressionSynchronizationContext).Execute(inputMemoryStream, outputStream, compressionOptions);
                while (!compressionSynchronizationContext.WaitCompletion(-1))
                {
                }
            }
            var readPosition = 0;

            for (var i = 0; i < 3; i++)
            {
                var headerBytes = outputByteArray.Skip(readPosition).Take(ChunkHeaderSize).ToArray();
                readPosition += BitConverter.ToInt32(headerBytes, 0) + ChunkHeaderSize;
            }

            // assert
            Assert.Equal(0, outputByteArray[readPosition]);
        }

        [Fact]
        public void Execute_BytesLessThanHeaderSizeDecompress_ExceptionRaised()
        {
            // arrange
            var inputByteArray = new byte[ChunkHeaderSize - 1];
            new Random().NextBytes(inputByteArray);
            var inputMemoryStream = new MemoryStream(inputByteArray);
            var outputByteArray = new byte[100];
            var compressionSynchronizationContext = new CompressionSynchronizationContext(1);
            var compressionOptions = new CompressionOptions { CompressionMode = CompressionMode.Decompress, ReadBufferSize = ChunkSize };

            // act
            using (var outputStream = new MemoryStream(outputByteArray))
            {
                GetCompressionProvider(compressionSynchronizationContext).Execute(inputMemoryStream, outputStream, compressionOptions);
                while (!compressionSynchronizationContext.WaitCompletion(-1))
                {
                }
            }

            var error = compressionSynchronizationContext.GetExceptions.ToArray()[0];

            // assert
            Assert.True(compressionSynchronizationContext.ExceptionsOccured);
            Assert.Equal(typeof(UserException), error.GetType());
            Assert.Equal(InvalidDataStreamMessage, error.Message);
        }

        [Fact]
        public void Execute_NegativeNumberHeader_ExceptionRaised()
        {
            // arrange
            var negativeHeader = BitConverter.GetBytes(-1);
            var inputByteArray = new byte[12];
            new Random().NextBytes(inputByteArray);
            inputByteArray = negativeHeader.Concat(inputByteArray).ToArray();
            var inputMemoryStream = new MemoryStream(inputByteArray);
            var outputByteArray = new byte[100];
            var compressionSynchronizationContext = new CompressionSynchronizationContext(1);
            var compressionOptions = new CompressionOptions { CompressionMode = CompressionMode.Decompress, ReadBufferSize = ChunkSize };

            // act
            using (var outputStream = new MemoryStream(outputByteArray))
            {
                GetCompressionProvider(compressionSynchronizationContext).Execute(inputMemoryStream, outputStream, compressionOptions);
                while (!compressionSynchronizationContext.WaitCompletion(-1))
                {
                }
            }

            var error = compressionSynchronizationContext.GetExceptions.ToArray()[0];

            // assert
            Assert.True(compressionSynchronizationContext.ExceptionsOccured);
            Assert.Equal(typeof(UserException), error.GetType());
            Assert.Equal(InvalidDataStreamMessage, error.Message);
        }

        private ICompressionProvider GetCompressionProvider(ICompressionSynchronizationContext compressionSynchronizationContext)
        {
            return new CompressionProvider(
                new GZipCompressor(),
                compressionSynchronizationContext,
                new ExecutionSafeContext(compressionSynchronizationContext));
        }
    }
}
