using System.IO.Compression;

namespace GZipCompressionTool.Core.Models
{
    public class ApplicationSettings
    {
        public CompressionMode CompressionMode { get; set; }

        public string InputFileFullName { get; set; }

        public string OutputFileFullName { get; set; }

        public int ChunkSize { get; set; }

        public int ProcessorsCount { get; set; }
    }
}
