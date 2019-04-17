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
        [HideInInspector] public string typeString;
        [HideInInspector] public string dataTypeString;
        [HideInInspector] public string variableKey;


        [HideInInspector] public NTVariableData data;
        [HideInInspector] public NTVariable _myData;

        public readonly string variableField = "variable";

        private Type variableType;
        private Type dataType;
        private MethodInfo getValueMethod;


        public string variablePath = "";

        public override IEnumerator ExecuteNode(NodeExecutionContext context){
            NTGraph g = graph as NTGraph;

            if(string.IsNullOrEmpty(typeString)) yield break;
            if(variableType == null || dataType == null) InitializeNodeTypes();
            if(_myData == null) _myData.FromNTVariableData(data);

            object portValue = GetPort(variableField).GetInputValue();
            object value = null;

            if(portValue == null){
                value = _myData.GetValue();
            }
            else
            {
                if(!string.IsNullOrEmpty(variablePath)){
                    value = g.sceneVariables.variableRepository.GetNTValue(variableKey, variableType);
                    
                    if(value == null) yield break;

                    ReflectionUtilities.SetValueOf(ref value, portValue, variablePath.Split('/').ToList());
                }
                else
                {
                    value = portValue;
                }
            }

            g.sceneVariables.variableRepository.SetValue(variableType,variableKey, value);
            yield return null;
        }

        public string GetVariableKey()
        {
            return variableKey;
        }

        public void SetVariableKey(string v,Type ntvaribaleType, string path, Type dataType = null)
        {
            variableKey = v;
            data.Name = variableKey;
            variablePath = path;

            if(dataType != null){
                dataTypeString = dataType.AssemblyQualifiedName;
            }

            if(!typeof(NTVariable).IsAssignableFrom(ntvaribaleType) || ntvaribaleType.IsGenericTypeDefinition) return;

            if(typeString != null) Debug.LogWarning("Trying to reporpouse a node...");

            typeString = ntvaribaleType.AssemblyQualifiedName;
            variableType = ntvaribaleType;

            InitializeNodeTypes();
        }

        private void InitializeNodeTypes(){
            NTGraph g = (NTGraph) graph;
            _myData = ((NTVariable) Activator.CreateInstance(variableType));
            data.Name = variableKey;

            if(!string.IsNullOrEmpty(dataTypeString)){
                dataType = Type.GetType(dataTypeString);
            }
            else
            {            
                dataType = _myData.GetDataType();
                dataTypeString = dataType.AssemblyQualifiedName;                
            }

            if(!HasPort(variableField)){
                AddInstanceInput(dataType, ConnectionType.Override, TypeConstraint.Strict, variableField);
            }
            //Need data type
            MethodInfo method = GetType().GetMethod("GetInputValue");
            getValueMethod = method.MakeGenericMethod(dataType);
        }

        public Type GetVariableType()
        {
            if(string.IsNullOrEmpty(typeString)) return GetType();
            if(variableType == null)variableType = Type.GetType(typeString);

            if(dataType == null) InitializeNodeTypes();

            return variableType;
        }

        public Type GetDataType()
        {
            if(string.IsNullOrEmpty(typeString)) return GetType();
            if(variableType == null)variableType = Type.GetType(typeString);

            if(dataType == null) InitializeNodeTypes();

            return dataType;
        }

    }
}