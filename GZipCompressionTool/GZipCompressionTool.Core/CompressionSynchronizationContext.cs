using System;
using System.Collections.Generic;
using GZipCompressionTool.Core.Interfaces;
using System.Threading;

namespace GZipCompressionTool.Core
{
    public class CompressionSynchronizationContext : ICompressionSynchronizationContext
    {
        public CompressionSynchronizationContext(int threadsCount)
        {
            _threadsRunningCount = threadsCount;
        }

        private EventWaitHandle _readHandle => new EventWaitHandle(true, EventResetMode.AutoReset);

        private EventWaitHandle _writeHandle => new EventWaitHandle(true, EventResetMode.ManualReset);

        private EventWaitHandle _finishedHandle => new EventWaitHandle(false, EventResetMode.ManualReset);

        private ICollection<Exception> Exceptions { get; } = new List<Exception>();

        private long _readCount;

        private long _writeCount = 1;

        private int _threadsRunningCount;

        public long GetChunkId()
        {
            _readHandle.WaitOne();
            return Interlocked.Increment(ref _readCount);
        }

        public void OnReadStarted()
        {
            _readHandle.Set();
        }

        public void OnPreWrite(long chunkId)
        {
            if (chunkId != _writeCount)
            {
                _writeHandle.WaitOne();
            }
        }

        public void OnWriteStarted()
        {
            Interlocked.Increment(ref _writeCount);
            _writeHandle.Set();
        }
        
        public void OnThreadFinish()
        {
            Interlocked.Decrement(ref _threadsRunningCount);
            _readHandle.Set();

            if (_threadsRunningCount == 0)
            {
                _finishedHandle.Set();
            }
        }

        public void OnException(Exception exception)
        {
            _readHandle.Reset();
            _writeHandle.Reset();
            Exceptions.Add(exception);
        }

        public void WaitCompletion()
        {
            _finishedHandle.WaitOne();
        }
    }
}
