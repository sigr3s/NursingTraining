using UnityEngine;

namespace NT.Nodes.Variables
{

    public interface IVariableNode {
        string GetVariableKey();
        void SetVariableKey(string v);
    }
    
}