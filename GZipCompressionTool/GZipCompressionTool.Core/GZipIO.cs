using GZipCompressionTool.Core.Interfaces;
using System;
using System.IO.Compression;

namespace GZipCompressionTool.Core
{
    public class GZipIO : IGZipIO
    {
        private IExecutionContext _executionContext;

        public IAsyncResult ReadGZip(int bufferSize, AsyncCallback asyncCallback)
        {
            return _executionContext.InputStream.BeginRead(_executionContext.Chunk.Payload, 0, bufferSize, asyncCallback, _executionContext);
        }

        public int GetCompressedChunkSize(int headerSize, byte[] chunkSize)
        {
            return _executionContext.InputStream.Read(chunkSize, 0, headerSize);
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

        public void SetExecutionContext(IExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }
    }
}
