using System;
using System.IO.Compression;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IGZipWriter
    {
        void WriteGZip(CompressionMode compressionMode, AsyncCallback asyncCallback);
    }
}
