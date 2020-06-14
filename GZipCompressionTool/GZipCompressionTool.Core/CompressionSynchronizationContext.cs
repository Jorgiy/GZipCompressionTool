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

        private long _chunkId;

        private long _nextToWriteChunkId = 1;

        private int _threadsRunningCount;

        private int _chunksToWriteCount;

        public ICollection<UserException> Exceptions { get; } = new List<UserException>();

        public long GetChunkId()
        {
            _readHandle.WaitOne();
            if (_errorHandle.WaitOne(0))
            {
                AbortCompress();
            }

            return Interlocked.Increment(ref _chunkId);
        }

        public void OnReadFinished()
        {
            _readHandle.Set();
        }

        public void OnPreWrite(long chunkId)
        {
            Interlocked.Increment(ref _chunksToWriteCount);

            while (chunkId != _nextToWriteChunkId)
            {
                _writeHandle.WaitOne(100);

                if (_errorHandle.WaitOne(0))
                {
                    Interlocked.Increment(ref _nextToWriteChunkId);
                    _writeHandle.Set();
                    AbortCompress();
                }
            }
        }

        public void OnWriteFinish()
        {
            Interlocked.Increment(ref _nextToWriteChunkId);
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

        public void OnException(UserException exception)
        {
            Exceptions.Add(exception);
            _errorHandle.Set();
            OnThreadFinish();
            _writeHandle.Set();
        }

        public bool ExceptionsOccured => _errorHandle.WaitOne(0);

        public IEnumerable<UserException> GetExceptions => Exceptions;

        public bool WaitCompletion(int milliseconds)
        {
            return (_allChunksAreReadHandle.WaitOne(milliseconds) && _chunksToWriteCount == _nextToWriteChunkId - 1) ||
                   (_errorHandle.WaitOne(milliseconds) && _chunksToWriteCount == _nextToWriteChunkId);
        }

        private void AbortCompress()
        {
            throw new CompressAbortedException();
        }
    }
}
