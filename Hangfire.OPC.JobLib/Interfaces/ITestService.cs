using Hangfire;
using OPCFoundation.ServerLib.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OPCFoundation.ServerLib.Interfaces
{
    public interface ITestService
    {
        bool RunTest(Guid id, JobType Type, CancellationToken cancellationToken);

        Task LongRunningMethod(CancellationToken cancellationToken);
    }
}
