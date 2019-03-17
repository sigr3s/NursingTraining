
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NT.Nodes.Messages;
using NT.Variables;
using UnityEditor;
using UnityEngine;
using XNode;
using XNode.InportExport;

namespace  NT.Graph
{
    public class NTGraph : NodeGraph {
        [Header("Execution Flow Nodes")]
        public List<NTNode> executionNodes;

        public List<CallbackNode> callbackNodes;

        public string loadFile = "ImportGraph.json";


        [Header("References")]
        public SceneVariables sceneVariables;

        public Dictionary<string, List<CallbackNode>> callbackNodesDict;

        public override Node AddNode(System.Type type){
            Node node = base.AddNode(type);
            if(node is CallbackNode){
                callbackNodes.Add( (CallbackNode) node );
            }
            return node;
        }

        public virtual void GenerateCallbackDict()
        {
            callbackNodesDict = new Dictionary<string, List<CallbackNode> >();
            foreach(CallbackNode cn in callbackNodes){
                if(cn != null && !string.IsNullOrEmpty(cn.callbackKey) ){
                    List<CallbackNode> callbacksInKey = new List<CallbackNode>();

                    if(callbackNodesDict.ContainsKey(cn.callbackKey)){
                        callbacksInKey = callbackNodesDict[cn.callbackKey];
                        callbacksInKey.Add(cn);
                        callbackNodesDict[cn.callbackKey] = callbacksInKey;
                    }
                    else
                    {
                        callbacksInKey.Add(cn);
                        callbackNodesDict[cn.callbackKey] = callbacksInKey;
                    }
                }
            }
        }

        public virtual void MessageRecieved(string message)
        {
            Debug.Log("Message recieved!   " + message);
            if(!string.IsNullOrEmpty(message) && callbackNodesDict.ContainsKey(message)){
                List<CallbackNode> nodesToExecute = callbackNodesDict[message];
                foreach(CallbackNode cn in nodesToExecute){
                    CoroutineRunner.Instance.StartCoroutine(StartExecutionFlow(cn));
                }
            }
        }

        public IEnumerator StartExecutionFlow(CallbackNode callbackNode)
        {
            NodeExecutionContext nodeExecutionContext = new NodeExecutionContext{node = callbackNode};

            while(nodeExecutionContext.node != null){

                Debug.Log("Execute node:  " + nodeExecutionContext.node.name);
                nodeExecutionContext.node.Enter();

                yield return new YieldNode(nodeExecutionContext );

                Debug.Log("Finished node:  " + nodeExecutionContext.node.name);

                yield return new WaitForSeconds(1.25f);

                Debug.Log("Finished waiting?:  " + nodeExecutionContext.node.name);

                nodeExecutionContext.node.Exit();

                nodeExecutionContext = nodeExecutionContext.node.NextNode(nodeExecutionContext);
            }

            yield return null;
        }

        Dictionary<Type, Type> dataToNtVatiable = new Dictionary<Type, Type>();

        public Type GetVariableFor(Type t){
            if(dataToNtVatiable.ContainsKey(t)) return dataToNtVatiable[t];


            foreach(Type nodeType in ReflectionUtilities.variableNodeTypes){
                if(nodeType.IsGenericTypeDefinition){
                    continue;
                }

                Type dataType = ((NTVariable) Activator.CreateInstance(nodeType)).GetDataType();

                if(dataType == t){
                    dataToNtVatiable.Add(t, nodeType);
                    return nodeType;
                }
            }

            Debug.Log("Not found??");
            return null;
        }

        [ContextMenu("Export")]
        public void Export(){
            JSONImportExport jep = new JSONImportExport();
            jep.Export(this, Application.dataPath + "/" + name + ".json");
        }

        [ContextMenu("Import")]
        public void Import(){
            string path = Application.dataPath + "/" + loadFile;
            JSONImportExport jimp = new JSONImportExport();
            NTGraph g = (NTGraph) jimp.Import(path);

            if(g == null) return;

            Clear();

            nodes = g.nodes;
            callbackNodes = g.callbackNodes;
            sceneVariables = g.sceneVariables;
            sceneVariables.variableRepository.dictionary.OnAfterDeserialize();

            Debug.Log(g.sceneVariables.variableRepository.dictionary.keys.Count);

            foreach(Node n in nodes){
                n.graph = this;
            }

            
        }
    }
}
