
using NT.Atributes;
using System.Collections;

namespace NT.Nodes.Other {
    
    public class PassExercice : FlowNode {

        [NTInput] public int grade; 
        
        public object GetValue() {
            return GetInputValue<object>("grade");
        }

        public override string GetDisplayName(){
            return "Pass Exercise";
        }


        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            MessageSystem.SendMessage("Pass Session /" + (GetValue() != null ? GetValue() : grade));
            yield return null;
        }

    }
}
