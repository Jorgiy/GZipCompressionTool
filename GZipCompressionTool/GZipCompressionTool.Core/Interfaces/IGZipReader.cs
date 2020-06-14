using System;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IGZipReader
    {        
        IAsyncResult ReadGZip(int bufferSize, AsyncCallback asyncCallback);

        int GetCompressedChunkSize(int headerSize, out int chunkSize);
    }
}
