using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using System.IO;
using System.IO.Compression;

namespace GZipCompressionTool.Core
{
    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(Stream inputStream, Stream outputStream, CompressionMode compressionMode)
        {
            InputStream = inputStream;
            OutputStream = outputStream;
            CompressionMode = compressionMode;
            GZipIo = new GZipIO();
            GZipIo.SetExecutionContext(this);
        }

        public Stream InputStream { get; }

        public Stream OutputStream { get; }

        public Chunk Chunk { get; set; }

        public int ChunkSize { get; set; }

        public IGZipIO GZipIo { get; set; }

        public CompressionMode CompressionMode { get; set; }
    }
}
