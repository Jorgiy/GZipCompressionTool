using System.Collections.Generic;
using System.Threading;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IThreadPoolDispatcher
    {
        IEnumerable<Thread> GetThreads(int threadsCount, ThreadStart startAction);
    }
}
