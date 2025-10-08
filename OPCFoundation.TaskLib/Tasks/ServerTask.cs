using Opc.Ua.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace OPCFoundation.TaskLib.Tasks
{
    public static class ServerTask
    {
        public static async Task Launch(ApplicationInstance app, int msec, string taskname, CancellationTokenSource cancellationTokenSource)
        {
            var token = cancellationTokenSource.Token;
            await LongRunningTaskAsync(token, app, msec);
            token.ThrowIfCancellationRequested();
        }

        internal static async Task LongRunningTaskAsync(CancellationToken token, ApplicationInstance app, int msec)
        {
            do
            {  
                SubProcedure(app, msec);
            }
            while (!token.IsCancellationRequested);
        }

        internal static void SubProcedure(ApplicationInstance app, int msec)
        {
            // here goes the logic of the sub-procedure, for example, checking the server status
            Thread.Sleep(msec); 
        }
    }
}
