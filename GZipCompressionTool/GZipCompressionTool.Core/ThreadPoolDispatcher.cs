using System;
using GZipCompressionTool.Core.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace GZipCompressionTool.Core
{
    public class ThreadPoolDispatcher : IThreadPoolDispatcher
    {
        public IEnumerable<Thread> GetThreads(int threadsCount, ThreadStart startAction)
        {
            for (var i = 0; i < threadsCount; i++)
            {
                yield return new Thread(startAction);
            }
        }
    }
}
