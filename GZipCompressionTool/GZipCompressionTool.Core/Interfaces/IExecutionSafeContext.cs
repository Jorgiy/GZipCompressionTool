﻿using System;
using GZipCompressionTool.Core.Models;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IExecutionSafeContext
    {
        void ExecuteSafe(Action action, Action<UserException> errorAction);
    }
}
