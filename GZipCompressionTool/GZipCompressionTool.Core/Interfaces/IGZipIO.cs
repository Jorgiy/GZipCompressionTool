namespace GZipCompressionTool.Core.Interfaces
{
    public interface IGZipIO : IGZipReader, IGZipWriter
    {
        void SetExecutionContext(IExecutionContext executionContext);
    }
}
