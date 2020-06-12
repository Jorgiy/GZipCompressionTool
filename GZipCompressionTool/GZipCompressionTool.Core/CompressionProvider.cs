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

        public CompressionProvider(
            IGZipCompressor gZipCompressor,
            ICompressionSynchronizationContext compressionSynchronizationContext)
        {
            _compressionSynchronizationContext = compressionSynchronizationContext;
            _gZipCompressor = gZipCompressor;
        }

        public void Execute(Stream inputStream, Stream outputStream, CompressionOptions compressionOptions)
        {
            var executionContext = new ExecutionContext(
                    inputStream,
                    outputStream, 
                    compressionOptions.CompressionMode)
            {
                ChunkSize = compressionOptions.ReadBufferSize
            };
            
            BeginReadChunk(executionContext);
        }

        private void BeginReadChunk(IExecutionContext executionContext)
        {
            executionContext.Chunk = new Chunk();
            var chunkId = _compressionSynchronizationContext.GetChunkId();
            var chunkSize = executionContext.ChunkSize;

            if (executionContext.CompressionMode == CompressionMode.Decompress)
            {
                var chunkSizeBuffer = new byte[ChunkHeaderSize];
                var bufferSizeBytesRead = executionContext.GZipIo.GetCompressedChunkSize(ChunkHeaderSize, chunkSizeBuffer);
                if (bufferSizeBytesRead == 0)
                {
                    _compressionSynchronizationContext.OnThreadFinish();
                    return;
                }

                chunkSize = BitConverter.ToInt32(chunkSizeBuffer, 0);
            }

            executionContext.Chunk.Id = chunkId;
            executionContext.Chunk.Payload = new byte[chunkSize];

            executionContext.GZipIo.ReadGZip(chunkSize, FinishRead);
        }

        private void FinishRead(IAsyncResult asyncResult)
        {
            var executionContext = asyncResult.AsyncState as IExecutionContext;
            var bytesRead = executionContext.InputStream.EndRead(asyncResult);
            _compressionSynchronizationContext.OnReadStarted();

            var readPayload = executionContext.Chunk.Payload;
            if (bytesRead < executionContext.Chunk.Payload.Length)
            {
                Array.Resize(ref readPayload, bytesRead);
                executionContext.Chunk.Payload = readPayload;
            }

            if (bytesRead == 0)
            {
                _compressionSynchronizationContext.OnThreadFinish();
                return;
            }

            executionContext.Chunk.Payload = executionContext.CompressionMode == CompressionMode.Compress ?
                _gZipCompressor.Compress(executionContext.Chunk.Payload) :
                _gZipCompressor.Decompress(executionContext.Chunk.Payload);

            BeginWrite(executionContext);
        }

        private void BeginWrite(IExecutionContext executionContext)
        {
            _compressionSynchronizationContext.OnPreWrite(executionContext.Chunk.Id);
            executionContext.GZipIo.WriteGZip(executionContext.CompressionMode, FinishWrite);
        }

        private void FinishWrite(IAsyncResult asyncResult)
        {
            var executionContext = asyncResult.AsyncState as IExecutionContext;
            executionContext.OutputStream.EndWrite(asyncResult);
            _compressionSynchronizationContext.OnWriteFinish();
            BeginReadChunk(executionContext);
        }
    }
}
