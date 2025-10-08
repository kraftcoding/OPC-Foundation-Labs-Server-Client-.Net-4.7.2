using DB.ModelLib.Managers;
using Hangfire.OPC.Configuration.Logs;
using Hangfire.OPC.JobLib.Base;
using Hangfire.OPC.JobLib.Init;
using Opc.Ua;
using Opc.Ua.Configuration;
using OPCFoundation.ClientLib.Client;
using OPCFoundation.ClientLib.Helpers;
using OPCFoundation.TaskLib.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Hangfire.OPC.JobLib.Jobs
{
    public class SuscribeNodesClientJob : JobBase
    {
        public static string JobName = "OPC UA Subscription Client";
        public static ApplicationInstance application;        
        public static CancellationTokenSource tokenSrc;
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>  
        public static void Init(string configFile, string filesPath)
        {
            UaClient Client = null;

            #region variables

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

            Utils.Trace("Initiating job... {0}", JobName);
            TextBuffer.WriteLine(string.Format("Initiating... {0}", JobName));

            try
            {
                tokenSrc = new CancellationTokenSource();
                ProcessModelContext context = new ProcessModelContext();
                Client = new UaClient(p_baseAddressId, appName, appConfig, context, JobInit.GetConfigFilePath(configFile, filesPath));
                                
                Utils.Trace("Stablishin comunication with server...");
                TextBuffer.WriteLine("Stablishin comunication with server...");
                Client.ConnectEndPoint(p_useSecurity);
           
                Utils.Trace("Connected to: " + Client.m_session.Endpoint.EndpointUrl.ToString());
                TextBuffer.WriteLine(string.Format("Connected to: " + Client.m_session.Endpoint.EndpointUrl.ToString()));

                Utils.Trace("Getting node config...");
                TextBuffer.WriteLine("Getting node config...");
                string[] nodeIds = ConfigHelper.GetConfigValues(Client, ns);
                
                Utils.Trace("Creating subscriptions...");
                TextBuffer.WriteLine("Creating subscriptions...");
                foreach (var nodeId in nodeIds)
                    Client.CreateSubscription(nodeId, "NODE #" + nodeId + "#", subsDictionary, MonitoringMode.Reporting);

                string taskname = "SuscribeNodesClientTask";
                Utils.Trace("Launching task... {0}", taskname);
                TextBuffer.WriteLine(string.Format("Launching task... {0}", taskname));
                SuscribeNodesClientTask ClientTsk = new SuscribeNodesClientTask();
                ClientTsk.Launch(Client, 60000, tokenSrc);
            }
            catch (OperationCanceledException ex)
            {
                Utils.Trace("Task was cancelled by user");
                TextBuffer.WriteLine(string.Format("Task was cancelled by user"));
            }
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());
                TextBuffer.WriteLine(string.Format("Error: {0}", ex.ToString()));
                TextBuffer.WriteLine(string.Format("StacTrace: {0}", ex.StackTrace));

                Stop();
            }
            finally
            {
                Client?.DisconnectEndPoint();
                Utils.Trace("Program completed");
                TextBuffer.WriteLine("Program completed");
            }

        }

        public static void Stop()
        {
            try
            {
                Utils.Trace("Stoping JOB... {0}", JobName);
                TextBuffer.WriteLine(string.Format("Stoping... {0}", JobName));
                tokenSrc.Cancel();
                tokenSrc.Dispose();
                //application.Stop();
            }
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());
                TextBuffer.WriteLine(string.Format("Error: {0}", ex.ToString()));
                TextBuffer.WriteLine(string.Format("StacTrace: {0}", ex.StackTrace));
            }
        }
    }
}
