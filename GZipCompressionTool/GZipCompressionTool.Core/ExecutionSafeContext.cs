using System;
using System.IO;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Core
{
    public class ExecutionSafeContext : IExecutionSafeContext
    {
        private readonly ICompressionSynchronizationContext _compressionSynchronizationContext;

        public ExecutionSafeContext(ICompressionSynchronizationContext compressionSynchronizationContext)
        {
            _compressionSynchronizationContext = compressionSynchronizationContext;
        }

        public void ExecuteSafe(Action action, Action<UserException> errorAction)
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
                var errorText = ioException.HResult == HrErrorHandleDiskFull || ioException.HResult == HrErrorDiskFull
                    ? "not enough space"
                    : string.Empty;

                errorAction?.Invoke(new UserException(IoErrorMessage + errorText + $"({ioException.Message})"));
            }
            catch (InvalidDataException)
            {
                errorAction?.Invoke(new UserException(InvalidDataStreamMessage));
            }
            catch (Exception exception)
            {
                errorAction?.Invoke(new UserException(OtherErrorsMessage + exception.Message));
            }
        }
    }
}
