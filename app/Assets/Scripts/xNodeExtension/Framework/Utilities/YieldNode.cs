
using System.Collections;
using NT.Nodes;
using UnityEngine;

namespace NT
{
    public class YieldNode : CustomYieldInstruction
    {
        public override bool keepWaiting{
           get{
               return executing;
           }
        }

        private NodeExecutionContext context;
        private bool executing = false;
        
        public YieldNode(NodeExecutionContext context){
            executing = true;
            this.context = context;
            CoroutineRunner.Instance.StartCoroutine(TrackedCoroutine());
        }

        IEnumerator TrackedCoroutine(){
            yield return CoroutineRunner.Instance.StartCoroutine(context.node.ExecuteNode(context));
            executing = false;
        }
    }
}
