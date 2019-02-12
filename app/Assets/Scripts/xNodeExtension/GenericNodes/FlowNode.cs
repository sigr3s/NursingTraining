using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class FlowNode : NTNode
    {
        [Input(onlyMatchingTypes = true)] public NTNode flowIn;
        [Input(onlyMatchingTypes = true)] public float dataValue;

    }
}
