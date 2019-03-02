using XNode;
using NT.Atributes;
using NT.Variables;
using NT.Graph;
using UnityEngine;
using System;

namespace NT.Nodes.Variables
{
    [System.Serializable]
    public class GetNTVariableNode : NTNode, IVariableNode //<T,K> : NTNode, IVariableNode where K: INTVaribale
    {
        [HideInInspector] [SerializeField] private string typeString;
        [HideInInspector] [SerializeField] public string variableKey;

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
                return ntVariable;
            }

            return null;
        }

        public string GetVariableKey()
        {
            return variableKey;
        }

        public void SetVariableKey(string v)
        {
            variableKey = v;
        }
        
        private void InitializeNodeTypes(){
            NTGraph g = (NTGraph) graph;
            INTVaribale _myData = ((INTVaribale) Activator.CreateInstance(variableType));
            
            dataType = _myData.GetDataType();

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

        public void SetNTVariableType(Type t)
        {
            if(!typeof(INTVaribale).IsAssignableFrom(t) || t.IsGenericTypeDefinition) return;

            if(typeString != null) Debug.LogWarning("TRying to reporpouse a node...");

            typeString = t.AssemblyQualifiedName;
            variableType = t;

            InitializeNodeTypes();
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