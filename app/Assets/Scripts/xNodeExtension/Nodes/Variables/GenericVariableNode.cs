using XNode;
using NT.Atributes;
using NT.Variables;
using NT.Graph;
using UnityEngine;

namespace NT.Nodes.Variables{
    [System.Serializable]
    public class GenericVariableNode<T> : NTNode
    {
        [NTOutput] public T variable;

        [HideInInspector] public string variableKey;
    }
}