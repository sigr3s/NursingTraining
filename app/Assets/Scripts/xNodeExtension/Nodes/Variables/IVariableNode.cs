using System;
using NT.Variables;
using UnityEngine;

namespace NT.Nodes.Variables
{

    public interface IVariableNode {
        void SetVariableKey(string v, Type ntvaribaleType, string path = "",  Type dataType = null);
    }
}