using GZipCompressionTool.Core.Interfaces;
using System;
using System.IO;
using System.IO.Compression;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Core
{
    public class GZipIO : IGZipIO
    {
        private readonly IExecutionContext _executionContext;

        public GZipIO(IExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }

        public IAsyncResult ReadGZip(int bufferSize, AsyncCallback asyncCallback)
        {
            return _executionContext.InputStream.BeginRead(_executionContext.Chunk.Payload, 0, bufferSize, asyncCallback, _executionContext);
        }

        public int GetCompressedChunkSize(int headerSize, out int chunkSize)
        {
            var chunkSizeBytes = new byte[ChunkHeaderSize];
            var bytesRead = _executionContext.InputStream.Read(chunkSizeBytes, 0, headerSize);

            if (bytesRead > 0 && bytesRead < ChunkHeaderSize)
            {
                throw new InvalidDataException();
            }

            try
            {
                chunkSize = BitConverter.ToInt32(chunkSizeBytes, 0);
            }
            catch (ArgumentException)
            {
                throw new InvalidDataException();
            }

            return bytesRead;
        }

        public void WriteGZip(CompressionMode compressionMode, AsyncCallback asyncCallback)
        {
            byte[] payload;

            if (compressionMode == CompressionMode.Compress)
            {
                payload = _executionContext.Chunk.GetPayloadWithHeader();
            }
            else
            {
                payload = _executionContext.Chunk.Payload;
            }

            _executionContext.OutputStream.BeginWrite(payload, 0, payload.Length, asyncCallback, _executionContext);
        }
    }
}
