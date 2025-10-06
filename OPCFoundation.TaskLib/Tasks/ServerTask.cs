using Opc.Ua;
using Opc.Ua.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TasksLib.Tasks
{
    public static class ServerTask
    {
        public static async Task Launch(ApplicationInstance app, int msec, string taskname)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            Task.Run(() =>
            {
                Console.WriteLine("Task is RUNNING: " + taskname);                
                bool result = false;
                do
                {
                    Console.WriteLine("Enter 'true' to stop...");
                    bool.TryParse(Console.ReadLine(), out result);
                } while (!result);

                if (result) cancellationTokenSource.Cancel();
            });

            try
            {
                Utils.Trace("Starting long-running task...");
                await LongRunningTaskAsync(token, app, msec);
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

        internal static async Task LongRunningTaskAsync(CancellationToken token, ApplicationInstance app, int msec)
        {
            do
            {
                // Check if cancellation is requested
                token.ThrowIfCancellationRequested();

                // ... call here the sub-procedure
                SubProcedure(app, msec);
            }
            while (!token.IsCancellationRequested);

            Utils.Trace("Task completed successfully");
        }

        internal static void SubProcedure(ApplicationInstance app, int msec)
        {

            //... here goes the logic of the sub-procedure
            Utils.Trace("Status: running");
            Thread.Sleep(msec); 
        }
    }
}
