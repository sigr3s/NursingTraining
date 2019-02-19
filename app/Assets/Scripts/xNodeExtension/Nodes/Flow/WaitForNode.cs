using System.Collections;
using NT.Atributes;
using UnityEngine;

namespace NT.Nodes.Flow{
    [System.Serializable]
    public class WaitForNode : FlowNode{
        [NTInput]  public int seconds;

        [NTInput] public DummyConnection breakFlow;

        private bool shouldBreakFlow = false;

        public override void Enter(){
            shouldBreakFlow = false;
            base.Enter();
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context){

            Debug.LogWarning(context.inputPort.fieldName);

            if(context.inputPort.fieldName == nameof(breakFlow)){
                Debug.LogWarning("Canceled wait node!");
                shouldBreakFlow = true;
            }
            else
            {
                int sec = GetInputValue<int>(nameof(this.seconds), this.seconds);
                float timeout = (float) sec;

                while(!shouldBreakFlow){
                    yield return null;
                    timeout -= Time.deltaTime;

                    if(timeout <= 0) break;
                }               
            }
        }

        public override NodeExecutionContext NextNode(NodeExecutionContext context){
            if(shouldBreakFlow){
                return new NodeExecutionContext() {node = null, inputPort = null, outputPort = GetPort(nameof(flowOut)) };
            }
            else
            {
                return base.NextNode(context);
            }
        }        
    }
}