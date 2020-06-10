using System;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface ICompressionSynchronizationContext
    {
        long GetChunkId();

        void OnReadStarted();

        void OnPreWrite(long chunkId);

        void OnWriteStarted();

        void OnWriteFinish(long chunkId);

        void OnThreadFinish();

        void OnException(Exception exception);

        void WaitCompletion();
    }
}
