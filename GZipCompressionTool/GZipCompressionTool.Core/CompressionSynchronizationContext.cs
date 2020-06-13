using System;
using System.Collections.Generic;
using GZipCompressionTool.Core.Interfaces;
using System.Threading;
using GZipCompressionTool.Core.Models;

namespace GZipCompressionTool.Core
{
    public class CompressionSynchronizationContext : ICompressionSynchronizationContext
    {
        public CompressionSynchronizationContext(int threadsCount)
        {
            _threadsRunningCount = threadsCount;
        }

        private readonly EventWaitHandle _readHandle = new EventWaitHandle(true, EventResetMode.AutoReset);

        private readonly EventWaitHandle _writeHandle = new EventWaitHandle(true, EventResetMode.ManualReset);

        private readonly EventWaitHandle _allChunksAreReadHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        private readonly EventWaitHandle _errorHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        private long _lastChunkId;

        private long _writeCount = 1;

        private int _threadsRunningCount;

        private int _chunksToWrite;

        public ICollection<Exception> Exceptions { get; } = new List<Exception>();

        public long GetChunkId()
        {
            _readHandle.WaitOne();
            if (_errorHandle.WaitOne(0))
            {
                AbortCompress();
            }

            return Interlocked.Increment(ref _lastChunkId);
        }

        public void OnReadFinished()
        {
            _readHandle.Set();
        }

        public void OnPreWrite(long chunkId)
        {
            Interlocked.Increment(ref _chunksToWrite);

            while (chunkId != _writeCount)
            {
                _writeHandle.WaitOne(100);

                if (_errorHandle.WaitOne(0))
                {
                    Interlocked.Increment(ref _writeCount);
                    _writeHandle.Set();
                    AbortCompress();
                }
            }
        }

        public void OnWriteFinish()
        {
            Interlocked.Increment(ref _writeCount);
            _writeHandle.Set();
            _writeHandle.Reset();
        }

        public void OnThreadFinish()
        {
            Interlocked.Decrement(ref _threadsRunningCount);
            _readHandle.Set();

            if (_threadsRunningCount == 0)
            {
                _allChunksAreReadHandle.Set();
            }
        }

        public void OnException(Exception exception)
        {
            Exceptions.Add(exception);
            _errorHandle.Set();
            OnThreadFinish();
            _writeHandle.Set();
        }

        public bool ExceptionsOccured => _errorHandle.WaitOne(0);

        public void WaitCompletion()
        {
            _allChunksAreReadHandle.WaitOne();

            while (_chunksToWrite != _writeCount - 1)
            {
                if (_errorHandle.WaitOne(100) && _chunksToWrite == _writeCount)
                {
                    return;
                }
            }
        }

        private void AbortCompress()
        {
            throw new CompressAbortedException();
        }
    }
}
