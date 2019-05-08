
using System;
using NT;
using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes.Variables
{
    [System.Serializable]
    public class GetVariableNode : NTNode
    {
        public readonly string variableField = "variable";
        [SerializeField] public string variableKey;

        [NTOutput] public object result;
        [NTOutput] public Type type;

        public void GetNTVariableNode(){
            if(!HasPort(variableField)){
                AddInstanceOutput(typeof(object), ConnectionType.Override, TypeConstraint.Strict, variableField);
            }
        }

        public override object GetValue(NodePort port) {
            return null;
        }


        public void SetVariableKey(string key){
            variableKey = key;
        }

    }

}