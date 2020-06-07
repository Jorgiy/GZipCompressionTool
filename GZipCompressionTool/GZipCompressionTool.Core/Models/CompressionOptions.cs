using System.IO.Compression;

namespace GZipCompressionTool.Core.Models
{
    public class CompressionOptions
    {
        public CompressionMode CompressionMode { get; set; }

        public int ReadBufferSize { get; set; }
    }
}
