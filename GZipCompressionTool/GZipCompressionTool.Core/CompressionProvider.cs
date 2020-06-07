using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using System.IO;

namespace GZipCompressionTool.Core
{
    public class CompressionProvider : ICompressionProvider
    {
        private readonly IGZipCompressor _gZipCompressor;

        private readonly ICompressionSynchronizationContext _compressionSynchronizationContext;

        private readonly IGZipIO _gZipIO;

        public CompressionProvider(
            IGZipCompressor gZipCompressor, 
            ICompressionSynchronizationContext compressionSynchronizationContext,
            IGZipIO gZipIO)
        {
            _compressionSynchronizationContext = compressionSynchronizationContext;
            _gZipCompressor = gZipCompressor;
            _gZipIO = gZipIO;
        }

        public void Execute(Stream inputStream, Stream outputStream, CompressionOptions compressionOptions)
        {
            var executionCOntext = new ExecutionContext(inputStream, outputStream) { Chunk = new Chunk() };

            _gZipIO.SetExecutionContext(executionCOntext);
        }
    }
}
