using System;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using System.IO;
using System.IO.Compression;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Core
{
    public class CompressionProvider : ICompressionProvider
    {
        private readonly IGZipCompressor _gZipCompressor;

        private readonly ICompressionSynchronizationContext _compressionSynchronizationContext;

        private readonly IGZipIO _gZipIo;

        public CompressionProvider(
            IGZipCompressor gZipCompressor, 
            ICompressionSynchronizationContext compressionSynchronizationContext,
            IGZipIO gZipIo)
        {
            _compressionSynchronizationContext = compressionSynchronizationContext;
            _gZipCompressor = gZipCompressor;
            _gZipIo = gZipIo;
        }

        public void Execute(Stream inputStream, Stream outputStream, CompressionOptions compressionOptions)
        {
            var executionContext = new ExecutionContext(
                    inputStream,
                    outputStream, 
                    compressionOptions.CompressionMode)
            {
                Chunk = new Chunk(), 
                ChunkSize = compressionOptions.ReadBufferSize
            };

            _gZipIo.SetExecutionContext(executionContext);

            BeginReadChunk(executionContext);
        }

        private void BeginReadChunk(IExecutionContext executionContext)
        {
            var chunkId = _compressionSynchronizationContext.GetChunkId();
            var chunkSize = executionContext.ChunkSize;

            if (executionContext.CompressionMode == CompressionMode.Decompress)
            {
                var chunkSizeBuffer = new byte[ChunkHeaderSize];
                var bufferSizeBytesRead = _gZipIo.GetCompressedChunkSize(ChunkHeaderSize, chunkSizeBuffer);
                if (bufferSizeBytesRead == 0)
                {
                    executionContext.CompressionSynchronizationContext.OnThreadFinish();
                    return;
                }

                chunkSize = BitConverter.ToInt32(chunkSizeBuffer, 0);
            }

            executionContext.Chunk.Id = chunkId;
            executionContext.Chunk.Payload = new byte[chunkSize];

            _gZipIo.ReadGZip(chunkSize, FinishRead);
            _compressionSynchronizationContext.OnReadStarted();
        }

        private void FinishRead(IAsyncResult asyncResult)
        {
            var executionContext = asyncResult as IExecutionContext;
            var bytesRead = executionContext.InputStream.EndRead(asyncResult);

            var readPayload = executionContext.Chunk.Payload;
            if (bytesRead < executionContext.Chunk.Payload.Length)
            {
                Array.Resize(ref readPayload, bytesRead);
            }

            if (bytesRead == 0)
            {
                executionContext.CompressionSynchronizationContext.OnThreadFinish();
                return;
            }

            executionContext.Chunk.Payload = executionContext.CompressionMode == CompressionMode.Compress ?
                _gZipCompressor.Compress(executionContext.Chunk.Payload) :
                _gZipCompressor.Decompress(executionContext.Chunk.Payload);

            BeginWrite(executionContext);
        }

        private void BeginWrite(IExecutionContext executionContext)
        {
            executionContext.CompressionSynchronizationContext.OnPreWrite(executionContext.Chunk.Id);
            _gZipIo.WriteGZip(executionContext.CompressionMode, FinishWrite);
            executionContext.CompressionSynchronizationContext.OnWriteStarted();
        }

        private void FinishWrite(IAsyncResult asyncResult)
        {
            var executionContext = asyncResult as IExecutionContext;
            executionContext.OutputStream.EndWrite(asyncResult);
            executionContext.Chunk = new Chunk();
            BeginReadChunk(executionContext);
        }
    }
}
