using System.Collections;
using System.Collections.Generic;
using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class FlowNode : NTNode
    {
        [NTInput] public DummyConnection flowIn;

    }
}
