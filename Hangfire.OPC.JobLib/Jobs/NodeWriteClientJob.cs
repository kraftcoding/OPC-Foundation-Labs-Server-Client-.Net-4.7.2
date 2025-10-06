using DB.ModelLib.Managers;
using OPCFoundation.ServerLib.Init;
using Opc.Ua;
using OPCFoundation.ClientLib.Client;
using OPCFoundation.ClientLib.Helpers;
using System;
using System.Collections.Generic;

namespace OPCFoundation.ServerLib.Jobs
{
    public static class NodeWriteClientJob
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>        
        public static void Init(string configFile)
        {
            #region Global variables

            //Params
            bool p_useSecurity = false;
            int p_baseAddressId = 0; //default 0
            string ns = "ns=2;s="; //default ns=2;s=
            string appName = "Client Test App 1";
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

            // create program controller
            ProcessModelContext context = new ProcessModelContext();
            UaClient Client = new UaClient(p_baseAddressId, appName, appConfig, context, JobInit.GetConfigFilePath(configFile));

            try
            {
                // Stablish comunication with server
                Client.ConnectEndPoint(p_useSecurity);
                Utils.Trace("Connected to: " + Client.m_session.Endpoint.EndpointUrl.ToString());                
                string[] nodeIds = ConfigHelper.GetConfigValues(Client, ns);                
                Launch(Client, nodeIds); 
            }
            catch (Exception exception)
            {
                Utils.Trace("Error: " + exception.ToString());
            }
            finally
            {
                Client.DisconnectEndPoint();
            }
        }

        public static void Launch(UaClient Client, string[] nodeIds)
        {
            try
            {                
                WriteNodes(Client, nodeIds);
            }            
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());
            }
        }      

        internal static void WriteNodes(UaClient Prg, string[] nodeIds)
        {
            int i = 0;
            foreach (var nodeId in nodeIds)
            {
                WriteValue(Prg, new NodeId(nodeId));
                i++;
            }            
        }

        // Get type of variable in OPC Server which should be written and cast the value before actually writing it        /
        internal static void WriteValue(UaClient Prg, NodeId nodeId)
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());
            }
        }
    }
}
