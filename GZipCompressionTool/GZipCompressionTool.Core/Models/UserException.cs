using System;

namespace GZipCompressionTool.Core.Models
{
    class UserException : Exception
    {
        public UserException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
