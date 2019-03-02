using System;
using System.Collections;
using System.Reflection;
using NT.Atributes;
using NT.Graph;
using NT.Variables;
using UnityEngine;
using XNode;

namespace NT.Nodes.Variables{
    [System.Serializable]
    public class SetNTVariableNode : FlowNode, IVariableNode // where K: INTVaribale
    {
        [HideInInspector] [SerializeField] private string typeString;
        [HideInInspector] [SerializeField] public string variableKey;
        public NTVariableData data;
        public INTVaribale _myData;

        public readonly string variableField = "variable";

        private Type variableType;
        private Type dataType;
        private MethodInfo getValueMethod;


        public override IEnumerator ExecuteNode(NodeExecutionContext context){
            NTGraph g = graph as NTGraph;

            if(string.IsNullOrEmpty(typeString)) yield break;
            if(variableType == null || dataType == null) InitializeNodeTypes();
            if(_myData == null) _myData.FromNTVariableData(data);

            var value = getValueMethod.Invoke(this, new object[] {variableField, _myData.GetValue()});

            g.sceneVariables.variableRepository.SetValue(variableType,variableKey, value);
            yield return null;
        }

        public string GetVariableKey()
        {
            return variableKey;
        }

        public void SetVariableKey(string v)
        {
            variableKey = v;
            data.Name = variableKey;
        }

        private void InitializeNodeTypes(){
            NTGraph g = (NTGraph) graph;
            _myData = ((INTVaribale) Activator.CreateInstance(variableType));
            data.Name = variableKey;
            
            dataType = _myData.GetDataType();

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

        public void SetNTVariableType(Type t)
        {
            if(!typeof(INTVaribale).IsAssignableFrom(t) || t.IsGenericTypeDefinition) return;

            if(typeString != null) Debug.LogWarning("TRying to reporpouse a node...");

            typeString = t.AssemblyQualifiedName;
            variableType = t;

            InitializeNodeTypes();
        }
    }

    public class VariableHelper<T,K> where K : INTVaribale{

    }

    public interface IGetHelper{

    }
}