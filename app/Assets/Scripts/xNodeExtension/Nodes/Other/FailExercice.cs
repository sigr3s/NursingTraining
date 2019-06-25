
using NT.Atributes;
using System.Collections;
using UnityEngine;

namespace NT.Nodes.Other {
    
    public class FailExercice : FlowNode {

        [NTInput] public int grade;

        public object GetValue() {
            return GetInputValue<object>("grade");
        }

        public override string GetDisplayName(){
            return "Fail Exercice";
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            MessageSystem.SendMessage("Fail Session /" + GetValue());
            yield return null;
        }
    }
}
