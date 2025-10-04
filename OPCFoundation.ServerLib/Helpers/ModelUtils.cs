using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCFoundation.ServerLib.Helpers
{
    public static class ModelUtils
    {
        /// <summary>
        /// The RootType for a Area node identfier.
        /// </summary>
        public const int Area = 0;

        /// <summary>
        /// The RootType for a Source node identfier.
        /// </summary>
        public const int Source = 1;

        /// <summary>
        /// Constructs a node identifier for a area.
        /// </summary>
        /// <param name="areaPath">The area path.</param>
        /// <param name="namespaceIndex">Index of the namespace that qualifies the identifier.</param>
        /// <returns>The new node identifier.</returns>
        public static NodeId ConstructIdForArea(string areaPath, ushort namespaceIndex)
        {
            ParsedNodeId parsedNodeId = new ParsedNodeId();

            parsedNodeId.RootId = areaPath;
            parsedNodeId.NamespaceIndex = namespaceIndex;
            parsedNodeId.RootType = 0;

            return parsedNodeId.Construct();
        }

        /// <summary>
        /// Constructs a NodeId for a source.
        /// </summary>
        /// <param name="sourceId">The source id.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        /// <returns>The new NodeId.</returns>
        public static NodeId ConstructIdForSource(string sourceId, ushort namespaceIndex)
        {
            ParsedNodeId parsedNodeId = new ParsedNodeId();

            parsedNodeId.RootId = sourceId;
            parsedNodeId.NamespaceIndex = namespaceIndex;
            parsedNodeId.RootType = 1;

            return parsedNodeId.Construct();
        }

        /// <summary>
        /// Constructs the node identifier for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        /// <returns>The node identifier for a component.</returns>
        public static NodeId ConstructIdForComponent(NodeState component, ushort namespaceIndex)
        {
            if (component == null)
            {
                return null;
            }

            // components must be instances with a parent.
            BaseInstanceState instance = component as BaseInstanceState;

            if (instance == null || instance.Parent == null)
            {
                return component.NodeId;
            }

            // parent must have a string identifier.
            string parentId = instance.Parent.NodeId.Identifier as string;

            if (parentId == null)
            {
                return null;
            }

            StringBuilder buffer = new StringBuilder();
            buffer.Append(parentId);

            // check if the parent is another component.
            int index = parentId.IndexOf('?');

            if (index < 0)
            {
                buffer.Append('?');
            }
            else
            {
                buffer.Append('/');
            }

            buffer.Append(component.SymbolicName);

            // return the node identifier.
            return new NodeId(buffer.ToString(), namespaceIndex);
        }
    }
}
