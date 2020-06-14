using System;

namespace GZipCompressionTool.Core.Models
{
    public class UserException : Exception
    {
        public UserException(string message) : base(message)
        {
        }
    }
}
