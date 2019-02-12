using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class FlowNode : NTNode
    {
        [Input] public Node flowIn;

    }
}
