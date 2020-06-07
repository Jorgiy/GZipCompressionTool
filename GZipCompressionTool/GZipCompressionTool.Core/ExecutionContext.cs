using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using System.IO;

namespace GZipCompressionTool.Core
{
    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(Stream inputStream, Stream outputStream)
        {
            InputStream = inputStream;
            OutputStream = outputStream;
        }

        public Stream InputStream { get; }

        public Stream OutputStream { get; }

        public Chunk Chunk { get; set; }
    }
}
