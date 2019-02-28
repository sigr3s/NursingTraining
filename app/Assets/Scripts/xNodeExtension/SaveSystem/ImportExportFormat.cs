using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XNode {
    public abstract class ImportExportFormat
	{
        public NodeGraph Import (string path){
			NodeGraphData data = ImportData (path);
			if (data == null)
				return null;
			return ConvertToNodeCanvas (data);
		}

		public void Export (NodeGraph graph, string path){
			NodeGraphData data = ConvertToNodeGraphData (graph);
			ExportData (data, path);
		}

        public abstract NodeGraphData ImportData (string path);

		public abstract void ExportData (NodeGraphData data, string path);


        public NodeGraph ConvertToNodeCanvas(NodeGraphData data){
            return null;
        }

        public NodeGraphData ConvertToNodeGraphData(NodeGraph graph){
            NodeGraphData nodeGraphData = new NodeGraphData(graph);

            Dictionary<NodePort, NodePortData> portDatas = new Dictionary<NodePort, NodePortData>();

            foreach (Node node in graph.nodes)
			{
                // Create node data
				NodeData nodeData = new NodeData (node);
				nodeGraphData.nodes.Add (nodeData);

                foreach (NodePort nodePort in node.Ports)
				{
					NodePortData portData = new NodePortData(nodeData, nodePort);

					nodeData.ports.Add(portData);
					portDatas.Add(nodePort, portData);
				}

            }

            foreach (NodePortData portData in portDatas.Values)
			{
				foreach (NodePort conPort in portData.port.GetConnections())
				{
					NodePortData conPortData; // Get portData associated with the connection port
					if (portDatas.TryGetValue(conPort, out conPortData))
						nodeGraphData.RecordConnection(portData, conPortData);
				}
			}

            return nodeGraphData;
        }


    }
}