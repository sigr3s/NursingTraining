
using NT.Nodes;
using UnityEngine;

namespace NT
{
    public class YieldNode : CustomYieldInstruction
    {
        public override bool keepWaiting{
           get{
               return node.KeepWaiting();
           }
        }

        private NTNode node;
        public YieldNode(NTNode runningNode){
            node = runningNode;
            node.ExecuteNode();
        }
    }
}
