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

        private readonly string variableField = "variable";

        [SerializeField] private Type variableType;
        [SerializeField] private string variablePath;
        [SerializeField] private Type dataType;
        [SerializeField] private string dataKey;

        public override object GetValue(NodePort port) {
            

            Debug.LogError("Not implemented yet!");

            return null;
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
                AddInstanceOutput(variableType, ConnectionType.Override, TypeConstraint.Strict, variablePath);
            }
        }

    }
}