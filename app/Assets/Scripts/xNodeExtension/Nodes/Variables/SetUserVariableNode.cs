using System;
using System.Collections;
using NT;
using NT.Graph;
using UnityEngine;
using XNode;

namespace NT.Nodes.Variables
{
    [System.Serializable]
    public class SetUserVariableNode : FlowNode, IVariableNode
    {
        [SerializeField] private object backingValue;
        [SerializeField] private string dataKey;

        public override object GetInstancePortValue(string fieldName){
            if(HasPort(fieldName)){
                NodePort port = GetPort(fieldName);

                if(port.IsConnected){
                    return port.GetInputValue();
                }
                else
                {
                    return backingValue;
                }
            }
            else
            {
                return backingValue;
            }
        }

        public override void SetInstancePortValue(string fieldName, object value){
            backingValue = value;
        }


       

        public void SetVariableKey(string dataKey, Type dataType){
            this.dataKey = dataKey;

             if(!HasPort(dataKey)){
                AddInstanceInput(dataType, ConnectionType.Override, TypeConstraint.Strict, dataKey);
            }
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context){

            NTGraph g = graph as NTGraph;

            NodePort port = GetPort(dataKey);

            object portValue = port.GetInputValue();

            if(!port.IsConnected){
                portValue = backingValue;
            }

            if(portValue != null){
                g.variableDelegate.SetUserVariable(dataKey, portValue);
            }

            yield return null;
        }

        public void SetVariableKey(string v, Type ntvaribaleType, string path = "", Type dataType = null)
        {
            return;
        }
    }
}