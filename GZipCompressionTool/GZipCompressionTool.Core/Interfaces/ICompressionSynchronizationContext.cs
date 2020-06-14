using System;
using System.Collections.Generic;
using GZipCompressionTool.Core.Models;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface ICompressionSynchronizationContext
    {
        long GetChunkId();

        void OnReadFinished();

        void OnPreWrite(long chunkId);

        void OnWriteFinish();

        void OnThreadFinish();

        void OnException(UserException exception);

        bool ExceptionsOccured { get; }

        IEnumerable<UserException> GetExceptions { get; }

        bool WaitCompletion(int milliseconds);
    }
}
