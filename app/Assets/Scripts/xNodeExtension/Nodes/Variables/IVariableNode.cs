using System;
using NT.Variables;
using UnityEngine;

namespace NT.Nodes.Variables
{

    public interface IVariableNode {
        string GetVariableKey();
        void SetVariableKey(string v);
        Type GetVariableType();
        Type GetDataType();

        void SetNTVariableType(Type t);
    }
}