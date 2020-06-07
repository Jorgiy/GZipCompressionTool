using GZipCompressionTool.Core.Models;
using System.IO;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface ICompressionProvider
    {
        void Execute(Stream inputStream, Stream outputStream, CompressionOptions compressionOptions);
    }
}
