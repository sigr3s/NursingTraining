using System;
using System.Collections;
using System.Collections.Generic;
using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT{
    [System.Serializable]
    public class NTNode : Node
    {
        [HideInInspector] public bool hasError = false;
        [HideInInspector] public string error = "";

        public virtual NodeExecutionContext NextNode(NodeExecutionContext context){
            return new NodeExecutionContext{node = null, inputPort = null, outputPort = null};
        }

        public virtual IEnumerator ExecuteNode(NodeExecutionContext context){
            yield return null;
        }

        public virtual void Enter(){
            isExecuting = true;
        }

        public virtual void Exit(){
            isExecuting = false;
        }

        [HideInInspector] public bool isExecuting = false;
        public NTNode GetNode(string portName){
            NodePort nodePort = GetOutputPort(portName);

            if (!nodePort.IsConnected) {
				Debug.LogWarning("Node isn't connected");
				return null;
			}

			NTNode node = nodePort.Connection.node as NTNode;
            
            return node;
        }

        public override object GetValue(NodePort port) {
            return null;
        }


        public virtual Color GetColor(){
            return Color.white;
        }


        public virtual string GetDisplayName(){
            return GetType().Name;
        }
        public virtual List<string> GetPath(){
            string path = GetType().ToString().Replace("NT.Nodes.", "");
            path = path.Replace(".", "/");

            List<string> pathParts = new List<string>(path.Split('/') );

            pathParts[pathParts.Count - 1] = GetDisplayName();
            return pathParts;
        }
    }
}