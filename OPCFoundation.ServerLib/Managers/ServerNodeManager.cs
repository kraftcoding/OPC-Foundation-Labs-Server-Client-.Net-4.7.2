using OPCFoundation.ServerLib.Base;
using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OPCFoundation.ServerLib.Managers
{
    internal class ServerNodeManager : BaseNodeManager
    {

        #region Private Fields
        //private readonly ServerTestConfiguration m_configuration;
        private List<BaseDataVariableState> m_dynamicNodes;
        private Timer m_simulationTimer;
        private bool m_simulationEnabled = true;
        private UInt16 m_simulationInterval = 3000;
        private Opc.Ua.Test.DataGenerator m_generator;
        private ServerConfigManager m_configuration;
        
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public ServerNodeManager(IServerInternal server, ApplicationConfiguration configuration, bool LoadDefaults)
        :
            base(server, configuration, OPCFoundation.ServerLib.Model.Namespaces.UaServer)
        {

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<ServerConfigManager>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new ServerConfigManager();
            }

            m_configuration.LoadDefaults = LoadDefaults;

            m_dynamicNodes = new List<BaseDataVariableState>();
        }
        #endregion


        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out IList<IReference> references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }
                FolderState root = CreateFolder(null, "Temperature Sensor", "Temperature Sensor");
                root.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, root.NodeId));
                root.EventNotifier = EventNotifiers.SubscribeToEvents;
                List<BaseDataVariableState> variables = new List<BaseDataVariableState>();

                try
                {
                    if (m_configuration.LoadDefaults)
                    {
                        #region Scalar
                        //FolderState scalarFolder = CreateFolder(root, "Scalar Values", "Scalar Values");
                        //BaseDataVariableState scalarInstructions = CreateVariable(scalarFolder, "Scalar_Instructions", "Scalar_Instructions", DataTypeIds.String, ValueRanks.Scalar);                                
                        //scalarInstructions.Value = "A library of Read/Write Variables of all supported data-types.";
                        //variables.Add(scalarInstructions);
                        #endregion

                        #region Scalar Static
                        FolderState staticFolder = CreateFolder(root, "Scalar-Static (writable) variables", "Scalar-Static (writable) variables");
                        const string scalarStatic = "Scalar_Static_";
                        variables.Add(CreateVariable(staticFolder, scalarStatic + "Boolean", "Boolean", DataTypeIds.Boolean, ValueRanks.Scalar));
                        variables.Add(CreateVariable(staticFolder, scalarStatic + "Byte", "Byte", DataTypeIds.Byte, ValueRanks.Scalar));
                        variables.Add(CreateVariable(staticFolder, scalarStatic + "ByteString", "ByteString", DataTypeIds.ByteString, ValueRanks.Scalar));
                        variables.Add(CreateVariable(staticFolder, scalarStatic + "DateTime", "DateTime", DataTypeIds.DateTime, ValueRanks.Scalar));
                        variables.Add(CreateVariable(staticFolder, scalarStatic + "Double", "Double", DataTypeIds.Double, ValueRanks.Scalar));
                        //BaseDataVariableState decimalVariable = CreateVariable(staticFolder, scalarStatic + "Decimal", "Decimal", DataTypeIds.DecimalDataType, ValueRanks.Scalar);
                        // Set an arbitrary precision decimal value.
                        //BigInteger largeInteger = BigInteger.Parse("1234567890123546789012345678901234567890123456789012345");
                        //DecimalDataType decimalValue = new DecimalDataType
                        //{
                        //    Scale = 100,
                        //    Value = largeInteger.ToByteArray()
                        //};
                        //decimalVariable.Value = decimalValue;
                        //variables.Add(decimalVariable);
                        #endregion

                        #region Scalar Simulation
                        FolderState simulationFolder = CreateFolder(root, "Scalar-Simulated variables", "Scalar-Simulated variables");
                        const string scalarSimulation = "Scalar_Simulation_";
                        CreateDynamicVariable(simulationFolder, scalarSimulation + "Boolean", "Boolean", DataTypeIds.Boolean, ValueRanks.Scalar);
                        CreateDynamicVariable(simulationFolder, scalarSimulation + "Byte", "Byte", DataTypeIds.Byte, ValueRanks.Scalar);
                        CreateDynamicVariable(simulationFolder, scalarSimulation + "ByteString", "ByteString", DataTypeIds.ByteString, ValueRanks.Scalar);
                        CreateDynamicVariable(simulationFolder, scalarSimulation + "DateTime", "DateTime", DataTypeIds.DateTime, ValueRanks.Scalar);
                        CreateDynamicVariable(simulationFolder, scalarSimulation + "Double", "Double", DataTypeIds.Double, ValueRanks.Scalar);
                        CreateDynamicVariable(simulationFolder, scalarSimulation + "Duration", "Duration", DataTypeIds.Duration, ValueRanks.Scalar);

                        BaseDataVariableState intervalVariable = CreateVariable(simulationFolder, scalarSimulation + "Interval", "Interval", DataTypeIds.UInt16, ValueRanks.Scalar);
                        intervalVariable.Value = m_simulationInterval;
                        intervalVariable.OnSimpleWriteValue = OnWriteInterval;

                        BaseDataVariableState enabledVariable = CreateVariable(simulationFolder, scalarSimulation + "Enabled", "Enabled", DataTypeIds.Boolean, ValueRanks.Scalar);
                        enabledVariable.Value = m_simulationEnabled;
                        enabledVariable.OnSimpleWriteValue = OnWriteEnabled;
                        #endregion
                    }

                    foreach (var ServerNode in m_configuration.ServerNodes)
                    {
                        FolderState folder = CreateFolder(root, ServerNode.FolderName, ServerNode.FolderName);
                        if (!string.IsNullOrEmpty(ServerNode.Bool)) variables.Add(CreateVariable(folder, ServerNode.Bool, "Boolean", DataTypeIds.Boolean, ValueRanks.Scalar));
                        if (!string.IsNullOrEmpty(ServerNode.String)) variables.Add(CreateVariable(folder, ServerNode.String, "Boolean", DataTypeIds.String, ValueRanks.Scalar));
                        if (!string.IsNullOrEmpty(ServerNode.Byte)) variables.Add(CreateVariable(folder, ServerNode.Byte, "Byte", DataTypeIds.Byte, ValueRanks.Scalar));
                        if (!string.IsNullOrEmpty(ServerNode.ByteString)) variables.Add(CreateVariable(folder,ServerNode.ByteString, "ByteString", DataTypeIds.ByteString, ValueRanks.Scalar));
                        if (!string.IsNullOrEmpty(ServerNode.DateTime)) variables.Add(CreateVariable(folder,ServerNode.DateTime, "DateTime", DataTypeIds.DateTime, ValueRanks.Scalar));
                        if (!string.IsNullOrEmpty(ServerNode.Double)) variables.Add(CreateVariable(folder,ServerNode.Double, "Double", DataTypeIds.Double, ValueRanks.Scalar));
                    }                    
                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Error creating the address space.");
                }                

                // save in dictionary. 
                AddPredefinedNode(SystemContext, root);
                m_simulationTimer = new Timer(DoSimulation, null, 3000, 1000);
            }
        }

        //simulate the node attribute values to change it continuously
        private void DoSimulation(object state)
        {
            try
            {
                lock (Lock)
                {
                    foreach (BaseDataVariableState variable in m_dynamicNodes)
                    {
                        variable.Value = GetNewValue(variable);
                        variable.Timestamp = DateTime.UtcNow;
                        variable.ClearChangeMasks(SystemContext, false);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error doing simulation.");
            }
        }

        // Creates a new folder.
        private FolderState CreateFolder(NodeState parent, string path, string name)
        {
            FolderState folder = new FolderState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = ObjectTypeIds.FolderType,
                NodeId = new NodeId(path, NamespaceIndex),
                BrowseName = new QualifiedName(path, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                EventNotifier = EventNotifiers.None
            };
            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        // Createing a new variable in the node.
        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            BaseDataVariableState variable = new BaseDataVariableState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                NodeId = new NodeId(path, NamespaceIndex),
                BrowseName = new QualifiedName(path, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                DataType = dataType,
                ValueRank = valueRank,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                Historizing = false
            };
            variable.Value = GetNewValue(variable);
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }
            if (parent != null)
            {
                parent.AddChild(variable);
            }
            return variable;
        }

        private object GetNewValue(BaseVariableState variable)
        {
            if (m_generator == null)
            {
                m_generator = new Opc.Ua.Test.DataGenerator(null)
                {
                    BoundaryValueFrequency = 0
                };
            }
            object value = null;
            int retryCount = 0;
            while (value == null && retryCount < 10)
            {
                value = m_generator.GetRandom(variable.DataType, variable.ValueRank, new uint[] { 10 }, Server.TypeTree);
                retryCount++;
            }
            
            return value;
        }

        //creating cariable dynamicly( changing value of the variable dynamicly)
        private BaseDataVariableState CreateDynamicVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            BaseDataVariableState variable = CreateVariable(parent, path, name, dataType, valueRank);
            m_dynamicNodes.Add(variable);
            return variable;
        }

        //writing value on specific intervals
        private ServiceResult OnWriteInterval(ISystemContext context, NodeState node, ref object value)
        {
            try
            {
                m_simulationInterval = (UInt16)value;
                if (m_simulationEnabled)
                {
                    m_simulationTimer.Change(100, (int)m_simulationInterval);
                }
                return ServiceResult.Good;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Error writing Interval variable.");
                return ServiceResult.Create(e, StatusCodes.Bad, "Error writing Interval variable.");
            }
        }

        //enabling write service
        private ServiceResult OnWriteEnabled(ISystemContext context, NodeState node, ref object value)
        {
            try
            {
                m_simulationEnabled = (bool)value;

                if (m_simulationEnabled)
                {
                    m_simulationTimer.Change(100, (int)m_simulationInterval);
                }
                else
                {
                    m_simulationTimer.Change(100, 0);
                }

                return ServiceResult.Good;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Error writing Enabled variable.");
                return ServiceResult.Create(e, StatusCodes.Bad, "Error writing Enabled variable.");
            }
        }

    }
}
