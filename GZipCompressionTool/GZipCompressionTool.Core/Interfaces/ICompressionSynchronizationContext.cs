using System;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface ICompressionSynchronizationContext
    {
        long GetChunkId();

        void OnReadFinished();

        void OnPreWrite(long chunkId);

        void OnWriteFinish();

        void OnThreadFinish();

        void OnException(Exception exception);

        bool ExceptionsOccured { get; }

        void WaitCompletion();
    }
}
