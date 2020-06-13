using System;
using System.IO;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;

namespace GZipCompressionTool.Core
{
    public class ExecutionSafeContext : IExecutionSafeContext
    {
        private readonly ICompressionSynchronizationContext _compressionSynchronizationContext;

        public ExecutionSafeContext(ICompressionSynchronizationContext compressionSynchronizationContext)
        {
            _compressionSynchronizationContext = compressionSynchronizationContext;
        }

        public void ExecuteSafe(Action action, Action<Exception> errorAction)
        {
            try
            {
                if (_compressionSynchronizationContext.ExceptionsOccured)
                {
                    _compressionSynchronizationContext.OnThreadFinish();
                    return;
                }

                action?.Invoke();
            }
            catch (CompressAbortedException)
            {
                _compressionSynchronizationContext.OnThreadFinish();
            }
            catch (IOException ioException)
            {
                errorAction?.Invoke(new UserException("", ioException));
            }
            catch (Exception exception)
            {
                errorAction?.Invoke(new UserException("", exception));
            }
        }
    }
}
