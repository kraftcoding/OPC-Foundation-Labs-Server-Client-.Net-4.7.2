using Opc.Ua;
using OPCFoundation.ClientLib.Client;
using OPCFoundation.ClientLib.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using OPCFoundation.TaskLib.Base;

namespace TasksLib.Tasks
{
    public class WriteNodesTask : TaskBase
    {
        public async Task Launch(UaClient Client, string[] nodeIds, int msec, string taskname)
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
                await LongRunningTaskAsync(token, Client, nodeIds, msec);
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

        internal async Task LongRunningTaskAsync(CancellationToken token, UaClient Prg, string[] nodeIds, int msec)
        {
            do
            {
                // Check if cancellation is requested
                token.ThrowIfCancellationRequested();
                WriteNodes(Prg, nodeIds, msec);
            }
            while (!token.IsCancellationRequested);

            Utils.Trace("Task completed successfully");
        }

        internal void WriteNodes(UaClient Prg, string[] nodeIds, int msec)
        {
            int i = 0;
            foreach (var nodeId in nodeIds)
            {                
                WriteValue(Prg, new NodeId(nodeId));
                i++;
            }

            Thread.Sleep(msec);
        }

        // Get type of variable in OPC Server which should be written and cast the value before actually writing it
        // public Task<bool> WriteValue(ProgramManager Prg, string nodeName, string varName, ushort namespaceIndex, string value)
        internal void WriteValue(UaClient Prg, NodeId nodeId)
        {
            try
            {
                // NodeId nodeId = new NodeId($"{nodeName}.{varName}", namespaceIndex);

                // Read the node you want to write to
                DataValue nodeToWrIteTo = Prg.m_session.ReadValue(nodeId);                

                //Type type = GetSystemType(Prg.m_session, nodeId);

                ////// Get type of the specific variable you want to write 
                BuiltInType type = nodeToWrIteTo.WrappedValue.TypeInfo.BuiltInType;                

                ////// Get the corresponding C# datatype
                Type csType = Type.GetType($"System.{type}");

                ////// Cast the value
                var castedValue = Convert.ChangeType(DataTypesHelper.GetNewValue(type), csType);

                // Create a WriteValue object with the new value
                var writeValue = new WriteValue
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(castedValue))
                };               

                // Write the new value to the node
                // new RequestHeader() if needed
                Prg.m_session.Write(null, new WriteValueCollection { writeValue }, out StatusCodeCollection statusCodeCollection, out DiagnosticInfoCollection diagnosticInfo);

                // Check the results to make sure the write succeeded
                if (statusCodeCollection[0].Code != Opc.Ua.StatusCodes.Good)
                {
                    Utils.Trace("Error: failed to write data");
                }
                //else
                //{
                //    Utils.Trace($"Wrote value {castedValue} to node {nodeId}");
                //}
            }
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());              
            }
        }
    }
}
