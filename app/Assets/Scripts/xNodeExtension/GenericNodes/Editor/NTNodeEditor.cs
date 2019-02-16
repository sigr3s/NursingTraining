using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;

namespace NT.Nodes{

    [CustomNodeEditor(typeof(NTNode))]
    public class NTNodeEditor : NodeEditor {
        public override Color GetTint(){
            NTNode node = target as NTNode;

            if(node.isExecuting){
                return Color.red;
            }
            else
            {
                return base.GetTint();
            }
        }

    }
}