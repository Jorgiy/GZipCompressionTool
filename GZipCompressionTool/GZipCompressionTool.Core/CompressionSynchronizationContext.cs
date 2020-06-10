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

        private readonly EventWaitHandle _readHandle = new EventWaitHandle(true, EventResetMode.AutoReset);

        private readonly EventWaitHandle _writeHandle = new EventWaitHandle(true, EventResetMode.ManualReset);

        private readonly EventWaitHandle _chunkWriteHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        private readonly EventWaitHandle _allChunksAreReadHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        private long _lastChunkId;

        private long _writeCount = 1;

        private long _lastWrittenChunk;

        private int _threadsRunningCount;

        private int _chunksToWrite;

        public ICollection<Exception> Exceptions { get; } = new List<Exception>();

        public long GetChunkId()
        {
            _readHandle.WaitOne();
            return Interlocked.Increment(ref _lastChunkId);
        }

        public void OnReadStarted()
        {
            _readHandle.Set();
        }

        public void OnPreWrite(long chunkId)
        {
            Interlocked.Increment(ref _chunksToWrite);

            while (chunkId != _writeCount)
            {
                _writeHandle.WaitOne(100);
            }
        }

        public void OnWriteStarted()
        {
            Interlocked.Increment(ref _writeCount);
            _writeHandle.Set();
        }

        public void OnWriteFinish(long chunkId)
        {
            Interlocked.Increment(ref _lastWrittenChunk);
            _chunkWriteHandle.Set();
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
            _readHandle.Reset();
            _writeHandle.Reset();
            Exceptions.Add(exception);
        }

        public void WaitCompletion()
        {
            _allChunksAreReadHandle.WaitOne();

            while (_chunksToWrite != _lastWrittenChunk)
            {
                _chunkWriteHandle.WaitOne(100);
            }
        }
    }
}
