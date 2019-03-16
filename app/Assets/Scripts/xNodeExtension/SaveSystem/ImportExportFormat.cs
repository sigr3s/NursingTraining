using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XNode.InportExport {
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
			ExportData (data, path, new List<Type>());
		}

        public abstract NodeGraphData ImportData (string path);

		public abstract void ExportData (NodeGraphData data, string path, List<Type> referenceTypes);


        public NodeGraph ConvertToNodeCanvas(NodeGraphData data){
            NodeGraph g = data.graph;

			foreach(NodeData n in data.nodes){
				g.nodes.Add(n.node);
			}

			foreach(ConnectionData ncd in data.connections){
				
			}

			return g;
        }

        public NodeGraphData ConvertToNodeGraphData(NodeGraph graph){
            NodeGraphData nodeGraphData = new NodeGraphData(graph);

            Dictionary<NodePort, NodePortData> portDatas = new Dictionary<NodePort, NodePortData>();

            foreach (Node node in graph.nodes)
			{
				if(!node) continue;
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