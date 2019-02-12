using System.Collections.Generic;
using NT.Nodes;
using UnityEngine;
using XNode;

namespace NT.Graph{


    [CreateAssetMenu(fileName = "New Scene Graph", menuName = "NT/Scene Graph")]
    public class SceneGraph : NodeGraph
    {
        public NTNode current;

        public List<CallabackNode> callbackNodes;

    }
}