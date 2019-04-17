using XNode;
using NT.Atributes;
using NT.Variables;
using NT.Graph;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace NT.Nodes.Variables
{
    [System.Serializable]
    public class GetNTVariableNode : NTNode, IVariableNode //<T,K> : NTNode, IVariableNode where K: INTVaribale
    {
        [HideInInspector] [SerializeField] public string typeString;
        [HideInInspector] [SerializeField] public string variableKey;
        [HideInInspector] [SerializeField] public string dataTypeString;
        [HideInInspector] [SerializeField] public string variablePath = "";

        public readonly string variableField = "variable";
        private Type variableType;
        private Type dataType;


        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            NTVariableRepository repo = g.sceneVariables.variableRepository;

            if(string.IsNullOrEmpty(typeString)) return null;
            if(variableType == null) variableType = Type.GetType(typeString);

            var ntVariable = repo.GetValue(variableKey, variableType);

            if(ntVariable != null){
                if(!string.IsNullOrEmpty(variablePath)){
                    return ReflectionUtilities.GetValueOf(variablePath.Split('/').ToList(), ntVariable);
                }
                return ntVariable;
            }

            return null;
        }

        public string GetVariableKey()
        {
            return variableKey;
        }

        public void SetVariableKey(string v,  Type ntvaribaleType, string path, Type dataType = null)
        {
            variableKey = v;
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
            NTVariable _myData = ((NTVariable) Activator.CreateInstance(variableType));

            if(!string.IsNullOrEmpty(dataTypeString)){
                dataType = Type.GetType(dataTypeString);
            }
            else
            {            
                dataType = _myData.GetDataType();
                dataTypeString = dataType.AssemblyQualifiedName;                
            }

            if(!HasPort(variableField)){
                AddInstanceOutput(dataType, ConnectionType.Override, TypeConstraint.Strict, variableField);
            }
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