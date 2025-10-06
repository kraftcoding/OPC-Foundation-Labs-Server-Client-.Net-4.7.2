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
        public async Task Launch(UaClient Client, int msec, string taskname)
        {
            Client.m_context.RData.idProcess = this.ProcessId.ToString();

            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            
            Task.Run(() =>
            {
                Console.WriteLine("Task is RUNNING: " + taskname);
                Console.WriteLine("Process ID: " + this.ProcessId);
                
                bool result = false;
                
                do
                {
                    Console.WriteLine("Enter 'true' to stop...");
                    bool.TryParse(Console.ReadLine(), out result);
                } while (!result);
                
                if(result) cancellationTokenSource.Cancel();
            });

            try
            {
                Utils.Trace("Starting long-running task...");
                await LongRunningTaskAsync(token, Client, msec);
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
                // Check if cancellation is requested
                token.ThrowIfCancellationRequested();

                SubProcedure(Prg, msec);
            }
            while (!token.IsCancellationRequested);

            Utils.Trace("Task completed successfully");
        }

        internal void SubProcedure(UaClient Prg, int msec)
        {

            //... here goes the logic of the sub-procedure
            Prg.m_context.InsertReadingsData();
            Thread.Sleep(msec);
        }
    }
}
