
namespace NT.Nodes.Other {
    
    public class PassExercice : FlowNode {

        public int grade;        

        public object GetValue() {
            return GetInputValue<object>("input");
        }

        public override string GetDisplayName(){
            return "Pass Exercice";
        }
    }
}
