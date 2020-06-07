using GZipCompressionTool.Core.Models;
using System.IO;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IExecutionContext
    {
        Stream InputStream { get; }

        Stream OutputStream { get; }

        Chunk Chunk { get; set; }
    }
}
