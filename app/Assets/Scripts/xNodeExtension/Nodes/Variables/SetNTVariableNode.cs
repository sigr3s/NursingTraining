using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NT.Atributes;
using NT.Graph;
using NT.Variables;
using UnityEngine;
using XNode;

namespace NT.Nodes.Variables{
    [System.Serializable]
    public class SetNTVariableNode : FlowNode, IVariableNode
    {
        private readonly string variableField = "variable";

        [SerializeField] private Type variableType;
        [SerializeField] private string variablePath;
        [SerializeField] private Type dataType;
        [SerializeField] private string dataKey;


        public override IEnumerator ExecuteNode(NodeExecutionContext context){
            
            NTGraph g = graph as NTGraph;
            
            object portValue = GetPort(variableField).GetInputValue();
            object value = null;


            if(portValue != null){
                if(!string.IsNullOrEmpty(variablePath)){
                    value = g.variableDelegate.GetValue(dataKey);
                    
                    if(value == null) yield break;

                    ReflectionUtilities.SetValueOf(ref value, portValue, variablePath.Split('/').ToList());

                    object v2 = g.variableDelegate.GetValue(dataKey);
                }
                else
                {
                    value = portValue;
                }
            }


            yield return null;
        }

        public void SetVariableKey(string dataKey,Type dataType, string variablePath, Type variableType){

            this.dataType = dataType;
            this.dataKey = dataKey;

            this.variablePath = variablePath;
            this.variableType = variableType;

            InitializeNodeTypes();
        }

        private void InitializeNodeTypes(){

            if(!HasPort(variablePath)){
                AddInstanceInput(variableType, ConnectionType.Override, TypeConstraint.Strict, variablePath);
            }
        }

    }
}