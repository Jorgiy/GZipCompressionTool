using System;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface ICompressionSynchronizationContext
    {
        long GetChunkId();

        void OnReadStarted();

        void OnPreWrite(long chunkId);

        void OnWriteFinish();

        void OnThreadFinish();

        void OnException(Exception exception);

        void WaitCompletion();
    }
}
