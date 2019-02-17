using NT.Atributes;
using NT.Variables;
using UnityEngine;
using XNode;

namespace NT.Nodes.Variables{
    [System.Serializable]
    public class SetGenericVariableNode<T> : FlowNode
    {
        [NTInput] public T value;

        [HideInInspector] public string variableKey;
    }
}