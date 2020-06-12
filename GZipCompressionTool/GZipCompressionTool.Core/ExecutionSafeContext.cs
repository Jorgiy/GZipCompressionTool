using System;
using GZipCompressionTool.Core.Interfaces;

namespace GZipCompressionTool.Core
{
    class ExecutionSafeContext : IExecutionSafeContext
    {
        private Action<Exception> _errorHandlingAction;

        public void ExecuteSafe(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception exception)
            {
                _errorHandlingAction?.Invoke(exception);
            }
        }

        public void SetErrorHandlingAction(Action<Exception> action)
        {
            _errorHandlingAction = action;
        }
    }
}
