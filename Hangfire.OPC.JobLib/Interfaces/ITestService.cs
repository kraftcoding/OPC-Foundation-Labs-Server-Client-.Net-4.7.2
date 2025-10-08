using Hangfire.OPC.Configuration.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hangfire.OPC.JobLib.Interfaces
{
    public interface ITestService
    {
        bool RunTest(Guid id, JobType Type, CancellationToken cancellationToken);

        Task LongRunningMethod(CancellationToken cancellationToken);
    }
}
