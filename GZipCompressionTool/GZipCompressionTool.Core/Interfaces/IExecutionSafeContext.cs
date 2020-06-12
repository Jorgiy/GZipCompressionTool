using System;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IExecutionSafeContext
    {
        void ExecuteSafe(Action action);

        void SetErrorHandlingAction(Action<Exception> action);
    }
}
