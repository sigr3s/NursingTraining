using System.Collections.Generic;
using System.IO;
using OdinSerializer;
using UnityEngine;
using XNode;

namespace  NT.Graph
{
    public class NodeGroupGraph : NodeGraph {
        public static string exportPath = Application.dataPath + "/Saves/NodeGroups/";
        public static DataFormat dataFormat = DataFormat.JSON;

        public List<NodePort> inputs = new List<NodePort>();
        public List<NodePort> outputs = new List<NodePort>();


        public static void Export(NodeGroupGraph graph){
            SerializationUtility.SerializeValue(graph, dataFormat);
        }

        public static List<NodeGroupGraph> GetAll(){
            List<NodeGroupGraph> groupedNodes = new List<NodeGroupGraph>();
            DirectoryInfo prefabsDir = new DirectoryInfo(NodeGroupGraph.exportPath);

            if(!prefabsDir.Exists) return null;

            FileInfo[] files = prefabsDir.GetFiles("*.nt");

            foreach(var file in files){
                byte[] nodeGroupData = File.ReadAllBytes(file.FullName);
                NodeGroupGraph po = SerializationUtility.DeserializeValue<NodeGroupGraph>(nodeGroupData, dataFormat);
                groupedNodes.Add(po);
            }
            return groupedNodes;
        }

        public static NodeGroupGraph GroupNodes(List<Node> nodesToGroup, NodeGraph g){
            if(nodesToGroup.Count < 2){
                Debug.LogWarning("Should be 2 or more nodes to make a group");
                return null;
            }
            foreach(var node in nodesToGroup){
                node.name = node.name + "_" + node.GetHashCode();
            }

            NodeGraph gcopy = g.Copy();            
            NodeGroupGraph nodeGroupGraph = new NodeGroupGraph();
            nodeGroupGraph.nodes = gcopy.nodes;

            for(int i = nodeGroupGraph.nodes.Count - 1; i >= 0; i--){
                var n = nodeGroupGraph.nodes[i];
                if(nodesToGroup.Find( no => no.name == n.name ) != null){
                    Debug.LogWarning("OK¿?");
                }
                else
                {
                    nodeGroupGraph.RemoveNode(n);
                }
            }

            foreach(var n in nodeGroupGraph.nodes){
                foreach(var p in n.Ports){
                    if(!p.IsConnected){
                        if(p.IsInput){
                            nodeGroupGraph.inputs.Add(p);
                        }
                        else
                        {
                            nodeGroupGraph.outputs.Add(p);
                        }
                    }
                }
            }

            Debug.Log("Inputs");

            foreach(var np in nodeGroupGraph.inputs){
                Debug.Log(" ___ INPUT " + np.fieldName);
            }

            Debug.Log("Outputs");

            foreach(var np in nodeGroupGraph.outputs){
                Debug.Log(" ___ OUT " + np.fieldName);
            }

            Debug.Log(nodeGroupGraph.nodes.Count);

            

        
            return nodeGroupGraph;
        }
    }
}