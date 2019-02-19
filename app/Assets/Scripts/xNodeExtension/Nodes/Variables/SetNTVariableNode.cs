using NT.Atributes;
using NT.Graph;
using NT.Variables;
using UnityEngine;
using XNode;

namespace NT.Nodes.Variables{
    [System.Serializable]
    public class SetNTVariableNode<T,K> : FlowNode, IVariableNode where K: INTVaribale
    {
        [NTInput] public T value;

        [HideInInspector] public string variableKey;

        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            T value = GetInputValue<T>(nameof(this.value), this.value);
            g.sceneVariables.variableRepository.SetValue<K>(variableKey, value);
        }

        public string GetVariableKey()
        {
            return variableKey;
        }

        public void SetVariableKey(string v)
        {
            variableKey = v;
        }
    }
}