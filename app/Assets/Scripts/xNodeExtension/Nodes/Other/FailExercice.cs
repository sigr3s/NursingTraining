
namespace NT.Nodes.Other {
    
    public class FailExercice : FlowNode {

        public int grade;        

        public object GetValue() {
            return GetInputValue<object>("input");
        }
    }
}
