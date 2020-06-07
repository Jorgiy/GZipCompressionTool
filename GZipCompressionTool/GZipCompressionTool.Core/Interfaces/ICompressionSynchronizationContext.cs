namespace GZipCompressionTool.Core.Interfaces
{
    public interface ICompressionSynchronizationContext
    {
        long GetChunkId();

        void OnRead();

        void OnWrite(long chunkId);

        void OnWriteFinished();

        void WaitCompletion();
    }
}
