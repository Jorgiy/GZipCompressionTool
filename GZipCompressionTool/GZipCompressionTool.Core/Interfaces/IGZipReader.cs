using System;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IGZipReader
    {        
        IAsyncResult ReadGZip(int bufferSize, AsyncCallback asyncCallback);

        IAsyncResult GetCompressedChunkSize(int headerSize, AsyncCallback asyncCallback, byte[] chunkSize);
    }
}
