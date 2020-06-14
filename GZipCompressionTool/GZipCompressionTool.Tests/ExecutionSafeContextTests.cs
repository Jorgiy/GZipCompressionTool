using GZipCompressionTool.Core;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using Moq;
using Xunit;

namespace GZipCompressionTool.Tests
{
    public class ExecutionSafeContextTests
    {
        [Fact]
        public void ExecuteSafe_ErrorsOccuredInAnotherThreads_OnOnThreadFinishCalled()
        {
            // arrange
            var compressionSynchronizationContextFakeObject = new Mock<ICompressionSynchronizationContext>();
            compressionSynchronizationContextFakeObject.Setup(fake => fake.ExceptionsOccured).Returns(true);
            var executionSafeContext = new ExecutionSafeContext(compressionSynchronizationContextFakeObject.Object);

            // act
            executionSafeContext.ExecuteSafe(() => {}, exception => { });

            // assert
            compressionSynchronizationContextFakeObject.Verify(fake => fake.ExceptionsOccured, Times.Once);
        }

        [Fact]
        public void ExecuteSafe_CallRaisesCompressAbort_OnOnThreadFinishCalled()
        {
            // arrange
            var compressionSynchronizationContextMock = new Mock<ICompressionSynchronizationContext>();;
            var executionSafeContext = new ExecutionSafeContext(compressionSynchronizationContextMock.Object);

            // act
            executionSafeContext.ExecuteSafe(() => throw new CompressAbortedException(), exception => { });

            // assert
            compressionSynchronizationContextMock.Verify(fake => fake.ExceptionsOccured, Times.Once);
        }
    }
}
