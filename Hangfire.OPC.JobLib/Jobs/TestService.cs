using Hangfire;
using OPCFoundation.ServerLib.Interfaces;
using OPCFoundation.ServerLib.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OPCFoundation.ServerLibLib.Jobs
{
    public static class TestService
    {     

        public static bool RunTest(Guid id, JobType jobType, CancellationToken token)
        {
            try
            {
                //cancellationToken.ThrowIfCancellationRequested();

                // ...
                //Thread.Sleep(120000);
                LongRunningMethod(token);
                // HERE IS THE PLACE TO DO THE JOB
                // ...
                // 
                return true;
            }
            catch (OperationCanceledException exception)
            {
                token.ThrowIfCancellationRequested();
                throw;
            }
            catch (Exception exception)
            {                
                throw;
            }
        }

        public static async Task LongRunningMethod(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(5000, token);
            }
            token.ThrowIfCancellationRequested();
        }

    }
}