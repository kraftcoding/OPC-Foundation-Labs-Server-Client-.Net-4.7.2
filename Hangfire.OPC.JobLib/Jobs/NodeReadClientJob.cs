using DB.ModelLib.Managers;
using Hangfire.OPC.Configuration.Logs;
using Hangfire.OPC.JobLib.Base;
using Hangfire.OPC.JobLib.Init;
using Opc.Ua;
using OPCFoundation.ClientLib.Client;
using OPCFoundation.ClientLib.Helpers;
using System;
using System.Collections.Generic;

namespace Hangfire.OPC.JobLib.Jobs
{
    public class NodeReadClientJob : JobBase
    {
        public static string JobName = "OPC UA Node Read Client";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>  
        public static void Init(string configFile, string filesPath)
        {
            #region Global variables

            //Params
            bool p_useSecurity = false;
            int p_baseAddressId = 0; //default 0
            string ns = "ns=2;s="; //default ns=2;s=
            string appName = "Client Test App 2";
            string appConfig = "ClientTestAppConfig";

            //Test subscription
            Dictionary<string, string> subsDictionary = new Dictionary<string, string>();
            subsDictionary.Add("DisplayName", "Alarm");
            subsDictionary.Add("PublishingInterval", "1000");
            subsDictionary.Add("KeepAliveCount", "10");
            subsDictionary.Add("LifetimeCount", "100");
            subsDictionary.Add("MaxNotificationsPerPublish", "1000");
            subsDictionary.Add("PublishingEnabled", "true");

            #endregion
                        
            TextBuffer.WriteLine(string.Format("Initiating... {0}", JobName));

            ProcessModelContext context = new ProcessModelContext();
            UaClient Client = new UaClient(p_baseAddressId, appName, appConfig, context, JobInit.GetConfigFilePath(configFile, filesPath));

            try
            {                
                TextBuffer.WriteLine("Stablishin comunication with server...");
                Client.ConnectEndPoint(p_useSecurity);
                
                TextBuffer.WriteLine(string.Format("Connected to: " + Client.m_session.Endpoint.EndpointUrl.ToString()));
             
                TextBuffer.WriteLine("Getting node config...");
                string[] nodeIds = ConfigHelper.GetConfigValues(Client, ns);

                string taskname = "ReadNodes";
             
                TextBuffer.WriteLine(string.Format("Launching task... {0}", taskname));
                Launch(Client, nodeIds);
            }
            catch (OperationCanceledException ex)
            {             
                TextBuffer.WriteLine(string.Format("Task was cancelled by user"));
            }
            catch (Exception ex)
            {                
                TextBuffer.WriteLine(string.Format("Error: {0}", ex.ToString()));
                TextBuffer.WriteLine(string.Format("StacTrace: {0}", ex.StackTrace));
            }
            finally
            {
                TextBuffer.WriteLine(string.Format("Disconnecting end-point"));
                Client.DisconnectEndPoint();

                TextBuffer.WriteLine(string.Format("Closing session"));
                Client.m_session?.Close();

                TextBuffer.WriteLine("Program completed");
            }
        }
    

        public static void Launch(UaClient Client, string[] nodeIds)
        {
            ReadNodes(Client, nodeIds);
        }        

        internal static void ReadNodes(UaClient Prg, string[] nodeIds)
        {
            List<NodeId> variableIds = new List<NodeId>();
            List<Type> expectedTypes = new List<Type>();

            List<object> values;
            List<ServiceResult> errors;

            foreach (var nodeId in nodeIds)
            {
                variableIds.Add(new NodeId(nodeId));
                expectedTypes.Add(null);
            }

            Prg.m_session.ReadValues(variableIds, expectedTypes, out values, out errors);

            int i = 0;
            foreach (var value in values)
            {
                if (errors[i].Code != Opc.Ua.StatusCodes.Good)
                {                    
                    TextBuffer.WriteLine("Error: failed to read data");
                }
                else
                {
                    NodeId node = variableIds[i];                 
                    TextBuffer.WriteLine(string.Format("Read value from node {0} is {1}", node, value));
                }                
                i++;
            }
        }
    }
}
