using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using System;

namespace XNode
{
    public class JSONImportExport : ImportExportFormat
    {
        public override void ExportData(NodeGraphData data, string path, List<Type> referenceTypes)
        {
            JSONObject exportJSON = new JSONObject();

            JSONObject graph = new JSONObject();

            JSONArray graphNodesReferences = new JSONArray();

            referenceTypes.Add(typeof(Node));
            referenceTypes.Add(typeof(NodeGraph));
            referenceTypes.Add(typeof(NodePort));

            graph.Add(  "type"  , new JSONString(data.graph.GetType().AssemblyQualifiedName) );
            graph.Add(  "name"  , new JSONString(data.graph.name)               );
            graph.Add(  "id"    , new JSONNumber(data.graph.GetHashCode())      );
            NodeGraph g = data.graph;
            graph = (JSONObject) SimpleJSONExtension.ToJSON(g, graph, new List<string>(), referenceTypes);

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
                nodeJSON.Add("id", node.GetHashCode());

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

                nodeJSON = (JSONObject) SimpleJSONExtension.ToJSON(node, nodeJSON, new List<string>(), referenceTypes);

                nodes.Add(nodeJSON);
            }

            exportJSON.Add("graph", graph);
            exportJSON.Add("conncetions", connections);
            exportJSON.Add("nodes", nodes);

            Debug.Log(exportJSON.ToString());

        }


        public override NodeGraphData ImportData(string path)
        {
            NodeGraphData returnData = null;

            if(File.Exists(path)){
                string jsonString =  File.ReadAllText(path);
                JSONNode root = JSON.Parse(jsonString);

                Dictionary<int, object> references = new Dictionary<int, object>();
                Dictionary<NodePort, NodePortData> portDatas = new Dictionary<NodePort, NodePortData>();

                if(root.HasKey("graph")){
                    JSONObject graphJObject = root["graph"].AsObject;
                    int id = graphJObject["id"];
                    string graphTypeS = graphJObject["type"];
                    Type graphType = Type.GetType(graphTypeS);

                    NodeGraph  graph = (NodeGraph) ScriptableObject.CreateInstance(graphType);
                    graph.name = graphJObject["name"];

                    references.Add(id, graph);
                    returnData = new NodeGraphData(graph);
                }
                else
                {
                    return returnData;
                }

                if(root.HasKey("nodes")){
                    JSONArray nodesJArray = root["nodes"].AsArray;

                    foreach (JSONObject nodeJObject in nodesJArray.Values)
                    {
                        int id = nodeJObject["id"];
                        string nodeTypeS = nodeJObject["type"];
                        Type nodeType = Type.GetType(nodeTypeS);

                        Node  node = (Node) ScriptableObject.CreateInstance(nodeType);
                        node.name = nodeJObject["name"];
                        node.graph = returnData.graph;
                        references.Add(id, node);
                        NodeData nodeData = new NodeData(node);

                        JSONArray nodePortsArray = nodeJObject["ports"].AsArray;
                        foreach (var nodePort in nodePortsArray.Values)
                        {
                            bool dynamic = nodePort["dynamic"].AsBool;
                            NodePort port = null;
                            int portId = 0;

                            if(dynamic){
                                Debug.LogWarning("Dynamic node ports not supported yet!");
                            }
                            else
                            {
                                port = node.GetPort(nodePort["name"]);
                                portId = nodePort["id"];
                            }

                            NodePortData portData = new NodePortData(nodeData, port);

                            nodeData.ports.Add(portData);
                            portDatas.Add(port, portData);
                            references.Add(portId, port);
                        }

                        returnData.nodes.Add(nodeData);
                    }

                }
                else
                {
                    return returnData;
                }

                Debug.Log("OK!");


                //Now connections are ok?
            }
            return null;
        }
    }
}