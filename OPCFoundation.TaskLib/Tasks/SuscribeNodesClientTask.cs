using Opc.Ua;
using OPCFoundation.ClientLib.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using OPCFoundation.TaskLib.Base;

namespace OPCFoundation.TaskLib.Tasks
{
    public partial class SuscribeNodesClientTask : TaskBase
    {
        public async Task Launch(UaClient client, int msec, CancellationTokenSource cancellationTokenSource)
        {
			client.m_context.RData.idProcess = this.TaskId.ToString();
            var token = cancellationTokenSource.Token;
			await LongRunningTaskAsync(token, client, msec);
			token.ThrowIfCancellationRequested();
		}

        internal async Task LongRunningTaskAsync(CancellationToken token, UaClient client, int msec)
        {
            do
            {
                SubProcedure(client, msec);
            }
            while (!token.IsCancellationRequested);
        }

        internal void SubProcedure(UaClient client, int msec)
        {
			client.m_context.InsertReadingsData();
			Thread.Sleep(msec);
        }
    }
}
