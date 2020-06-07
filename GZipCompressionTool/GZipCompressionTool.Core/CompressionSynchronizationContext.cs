using GZipCompressionTool.Core.Interfaces;
using System.Threading;

namespace GZipCompressionTool.Core
{
    public class CompressionSynchronizationContext : ICompressionSynchronizationContext
    {
        public CompressionSynchronizationContext(int threadsCount)
        {
            _threadsRunnigCount = threadsCount;
        }

        private EventWaitHandle _readHandle => new EventWaitHandle(false, EventResetMode.AutoReset);

        private EventWaitHandle _writeHandle => new EventWaitHandle(false, EventResetMode.ManualReset);

        private EventWaitHandle _finishedHandle => new EventWaitHandle(false, EventResetMode.ManualReset);

        private long _readCount { get; set; }

        private long _writeCount { get; set; }

        private int _threadsRunnigCount { get; set; }

        public long GetChunkId()
        {
            throw new System.NotImplementedException();
        }

        public void OnRead()
        {
            throw new System.NotImplementedException();
        }

        public void OnWrite(long chunkId)
        {
            throw new System.NotImplementedException();
        }

        public void OnWriteFinished()
        {
            throw new System.NotImplementedException();
        }

        public void WaitCompletion()
        {
            throw new System.NotImplementedException();
        }
    }
}
