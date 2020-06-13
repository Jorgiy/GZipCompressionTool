using System;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IExecutionSafeContext
    {
        void ExecuteSafe(Action action, Action<Exception> errorAction);
    }
}
