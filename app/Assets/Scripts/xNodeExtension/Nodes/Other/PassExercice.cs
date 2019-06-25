
using NT.Atributes;
using System.Collections;

namespace NT.Nodes.Other {
    
    public class PassExercice : FlowNode {

        [NTInput] public int grade; 
        
        public object GetValue() {
            return GetInputValue<object>("grade");
        }

        public override string GetDisplayName(){
            return "Pass Exercice";
        }


        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            MessageSystem.SendMessage("Pass Session /" +  (int) GetValue() );
            yield return null;
        }

    }
}
