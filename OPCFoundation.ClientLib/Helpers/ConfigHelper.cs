using Opc.Ua;
using OPCFoundation.ClientLib.Client;
using OPCFoundation.ServerLib.Managers;
using System.Collections;

namespace OPCFoundation.ClientLib.Helpers
{
    public static class ConfigHelper
    {
        public static string[] GetConfigValues(UaClient Client, string ns)
        {            
            ServerConfigManager m_configuration = Client.m_configuration.ParseExtension<ServerConfigManager>();
            ArrayList arrayList = new ArrayList();  
            if (m_configuration != null)
            {                
                foreach (var ServerNode in m_configuration.ServerNodes)
                {                    
                    if (!string.IsNullOrEmpty(ServerNode.Bool)) arrayList.Add(CreateId(ServerNode.Bool, ns));
                    if (!string.IsNullOrEmpty(ServerNode.String)) arrayList.Add(CreateId(ServerNode.String, ns));
                    if (!string.IsNullOrEmpty(ServerNode.Byte)) arrayList.Add(CreateId(ServerNode.Byte, ns));
                    if (!string.IsNullOrEmpty(ServerNode.ByteString)) arrayList.Add(CreateId(ServerNode.ByteString, ns));
                    if (!string.IsNullOrEmpty(ServerNode.DateTime)) arrayList.Add(CreateId(ServerNode.DateTime, ns));
                    if (!string.IsNullOrEmpty(ServerNode.Double)) arrayList.Add(CreateId(ServerNode.Double, ns));
                }
            }
            else
            {
                Helpers.Trace("No configuration found in local configuration file");
            }

            return (string[]) arrayList.ToArray(typeof(string));
        }

        public static string CreateId(string nodeName, string ns)
        {            
            return  ns + nodeName;
        }
    }
}
