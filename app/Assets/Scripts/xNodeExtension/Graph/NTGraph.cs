using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using NT.Nodes.Messages;
using NT.Variables;
using UnityEditor;
using UnityEngine;

using XNode;

namespace  NT.Graph
{
    public class NTGraph : NodeGraph {
        [Header("Execution Flow Nodes")]
        public List<NTNode> executionNodes = new List<NTNode>();
        public List<NodeGroupGraph> packedNodes = new List<NodeGroupGraph>();
        public IVariableDelegate variableDelegate;

        public virtual List<string> GetCallbacks()
        {
            return new List<string>();
        }

        public List<CallbackNode> callbackNodes = new List<CallbackNode>();

        public Dictionary<string, List<CallbackNode>> callbackNodesDict = new Dictionary<string, List<CallbackNode>>();

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
                if(cn != null && !string.IsNullOrEmpty(cn.GetCallbackKey()) ){
                    List<CallbackNode> callbacksInKey = new List<CallbackNode>();

                    if(callbackNodesDict.ContainsKey(cn.GetCallbackKey())){
                        callbacksInKey = callbackNodesDict[cn.GetCallbackKey()];
                        callbacksInKey.Add(cn);
                        callbackNodesDict[cn.GetCallbackKey()] = callbacksInKey;
                    }
                    else
                    {
                        callbacksInKey.Add(cn);
                        callbackNodesDict[cn.GetCallbackKey()] = callbacksInKey;
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

        public void AddGroupedNodes(NodeGroupGraph group)
        {
            packedNodes.Add(group);
        }

        public void RemoveGroupedNodes(NodeGroupGraph group)
        {
            for(int i = group.nodes.Count - 1; i >= 0; i--){
                group.RemoveNode(group.nodes[i]);
            }

            packedNodes.Remove(group);
        }

        public IEnumerator StartExecutionFlow(CallbackNode callbackNode)
        {
            NodeExecutionContext nodeExecutionContext = new NodeExecutionContext{node = callbackNode};

            while(nodeExecutionContext.node != null){

                Debug.Log("Execute node:  " + nodeExecutionContext.node);
                nodeExecutionContext.node.Enter();

                yield return new YieldNode(nodeExecutionContext );

                Debug.Log("Finished node:  " + nodeExecutionContext.node);

                yield return new WaitForSeconds(1.25f);

                Debug.Log("Finished waiting?:  " + nodeExecutionContext.node);

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

    }
}
