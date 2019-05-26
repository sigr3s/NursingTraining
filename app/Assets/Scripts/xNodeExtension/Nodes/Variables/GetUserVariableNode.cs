using System;
using NT;
using NT.Graph;
using UnityEngine;
using XNode;

namespace NT.Nodes.Variables
{
    [System.Serializable]
    public class GetUserVariableNode : NT.NTNode, IVariableNode
    {

        [SerializeField] private string dataKey;


        public override object GetValue(NodePort port) {
            if(graph is NTGraph){
                NTGraph g = (NTGraph) graph;
                object variableValue = g.variableDelegate.GetUserVariable(dataKey);

                if(variableValue != null){
                    return variableValue;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void SetVariableKey(string dataKey, Type dataType){
            this.dataKey = dataKey;

             if(!HasPort(dataKey)){
                AddInstanceOutput(dataType, ConnectionType.Override, TypeConstraint.Strict, dataKey);
            }
        }

        public void SetVariableKey(string v, Type ntvaribaleType, string path = "", Type dataType = null)
        {
            return;
        }
    }
}