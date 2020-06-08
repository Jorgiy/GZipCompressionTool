using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using System;
using System.IO;
using System.IO.Compression;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Core
{
    public class CompressionProvider : ICompressionProvider
    {
        private readonly IGZipCompressor _gZipCompressor;

        private readonly ICompressionSynchronizationContext _compressionSynchronizationContext;

        private readonly IGZipIO _gZipIO;

        public CompressionProvider(
            IGZipCompressor gZipCompressor, 
            ICompressionSynchronizationContext compressionSynchronizationContext,
            IGZipIO gZipIO)
        {
            _compressionSynchronizationContext = compressionSynchronizationContext;
            _gZipCompressor = gZipCompressor;
            _gZipIO = gZipIO;
        }

        public void Execute(Stream inputStream, Stream outputStream, CompressionOptions compressionOptions)
        {
            var executionCOntext = new ExecutionContext(inputStream, outputStream) { Chunk = new Chunk() };

            _gZipIO.SetExecutionContext(executionCOntext);


        }

        private void BeginReadChunk(CompressionOptions compressionOptions, ExecutionContext executionContext)
        {
            var chunkId = _compressionSynchronizationContext.GetChunkId();
            var chunkSize = compressionOptions.ReadBufferSize;

            if (compressionOptions.CompressionMode == CompressionMode.Decompress)
            {
                var chunkSizeBuffer = new byte[ChunkHeaderSize];
                _gZipIO.GetCompressedChunkSize(ChunkHeaderSize, GetCompressedChunkSize, chunkSizeBuffer);
            }
        }

        private void GetCompressedChunkSize(IAsyncResult asyncResult)
        {
        }
    }
}
