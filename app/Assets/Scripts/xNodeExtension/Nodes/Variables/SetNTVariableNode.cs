using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NT.Atributes;
using NT.Graph;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;
using XNode;

namespace NT.Nodes.Variables{
    [System.Serializable]
    public class SetNTVariableNode : FlowNode, IVariableNode
    {
        [SerializeField] private Type variableType;
        [SerializeField] private string variablePath;
        [SerializeField] private Type dataType;
        [SerializeField] private string dataKey;


        [SerializeField] private object backingValue;

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

        public override IEnumerator ExecuteNode(NodeExecutionContext context){

            NTGraph g = graph as NTGraph;

            NodePort port = GetPort(variablePath);

            object portValue = port.GetInputValue();

            if(!port.IsConnected){
                portValue = backingValue;
            }

            object value = null;


            if(portValue != null){

                if(!string.IsNullOrEmpty(variablePath)){
                    value = g.variableDelegate.GetValue(dataKey);

                    if(value == null) yield break;

                    ReflectionUtilities.SetValueOf(ref value, portValue, variablePath.Split('/').ToList());

                    g.variableDelegate.SetValue(dataKey, value);
                }
                else
                {
                    value = portValue;
                }
            }
            else
            {

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