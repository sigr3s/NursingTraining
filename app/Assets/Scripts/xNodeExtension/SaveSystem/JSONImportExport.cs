using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

namespace XNode
{
    public class JSONImportExport : ImportExportFormat
    {
        public override void ExportData(NodeGraphData data, string path)
        {
            JSONObject exportJSON = new JSONObject();

            JSONObject graph = new JSONObject();

            JSONArray graphNodesReferences = new JSONArray();

            for(int i = 0; i < data.graph.nodes.Count; i++){
                graphNodesReferences.Add("nodeID", new JSONNumber(data.graph.nodes[i].GetHashCode()));
            }

            graph.Add(  "type"  , new JSONString(data.graph.GetType().AssemblyQualifiedName) );
            graph.Add(  "name"  , new JSONString(data.graph.name)               );
            graph.Add(  "id"    , new JSONNumber(data.graph.GetHashCode())      );
            graph.Add(  "nodes" , graphNodesReferences);


            JSONArray connections = new JSONArray();

            for(int c = 0; c < data.connections.Count; c++){
                JSONObject connection = new JSONObject();
                connection.Add("port1ID", data.connections[c].port1ID);
                connection.Add("port2ID", data.connections[c].port2ID);
                connections.Add(connection);
            }

            JSONArray nodes = new JSONArray();

            for(int n = 0; n < data.nodes.Count; n++){
                Node node = data.nodes[n].node;
                JSONObject nodeJSON = new JSONObject();
                nodeJSON.Add("name" ,  node.name );
                nodeJSON.Add("type" , node.GetType().AssemblyQualifiedName);
                nodeJSON.Add("position", node.position);

                JSONArray nodePorts = new JSONArray();

                for(int np = 0; np < data.nodes[n].ports.Count; np++){
                    NodePort port = data.nodes[n].ports[np].port;

                    JSONObject nodePortJSON = new JSONObject();
                    nodePortJSON.Add("name", port.fieldName);
                    nodePortJSON.Add("id", port.GetHashCode());
                    nodePortJSON.Add("valueType", port.ValueType.AssemblyQualifiedName);
                    nodePortJSON.Add("dynamic", port.IsDynamic);
                    nodePortJSON.Add("typeConstraint", (int) port.typeConstraint);
                    nodePortJSON.Add("connectionType", (int) port.connectionType);
                    nodePortJSON.Add("direction", (int) port.direction);

                    nodePorts.Add(nodePortJSON);
                }

                nodeJSON.Add("ports", nodePorts);

                List<string> ignoeredFields = new List<string>{"graph", "position"};
                nodeJSON = (JSONObject) SimpleJSONExtension.ToJSON(node, nodeJSON, ignoeredFields);

                nodes.Add(nodeJSON);
            }

            exportJSON.Add("graph", graph);
            exportJSON.Add("conncetions", connections);
            exportJSON.Add("nodes", nodes);

            Debug.Log(exportJSON.ToString());

        }


        public override NodeGraphData ImportData(string path)
        {
            if(File.Exists(path)){
                string jsonString =  File.ReadAllText(path);
                JSONNode root = JSON.Parse(jsonString);

                if(root.HasKey("graph")){
                    Debug.Log("Graph??");
                }

                if(root.HasKey("nodes")){
                    Debug.Log("Nodes??");
                }
            }
            return null;
        }
    }
}