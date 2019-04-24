
namespace NT.Nodes.Other {
    
    public class PassExercice : FlowNode {

        public int grade;        

        public object GetValue() {
            return GetInputValue<object>("input");
        }
    }
}
