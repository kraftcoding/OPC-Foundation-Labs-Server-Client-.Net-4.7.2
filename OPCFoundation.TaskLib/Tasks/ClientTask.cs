using Opc.Ua;
using OPCFoundation.ClientLib.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using OPCFoundation.TaskLib.Base;

namespace TasksLib.Tasks
{
    public partial class ClientTask : TaskBase
    {
        public async Task Launch(UaClient Client, int msec, string taskname, CancellationTokenSource cancellationTokenSource)
        {
            Client.m_context.RData.idProcess = this.ProcessId.ToString();

            var token = cancellationTokenSource.Token;

            try
            {
                Utils.Trace("Starting long-running task...");
                await LongRunningTaskAsync(token, Client, msec);

                token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                Utils.Trace("Task was cancelled by user");
            }
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            Utils.Trace("Program completed");
        }

        internal async Task LongRunningTaskAsync(CancellationToken token, UaClient Prg, int msec)
        {
            do
            {
                SubProcedure(Prg, msec);
            }
            while (!token.IsCancellationRequested);

            Utils.Trace("Task completed successfully");
        }

        internal void SubProcedure(UaClient Prg, int msec)
        {
            Thread.Sleep(msec);
            Prg.m_context.InsertReadingsData();            
        }
    }
}
