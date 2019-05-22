using NT.Atributes;

namespace NT.Nodes.Other {
    
    public class CompareTypes : NTNode {

        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DummyConnection type01;
        public Tools tool;
        
        public enum Tools
        {
            Scissors01,
            Scissors02,
            Forceps01,
            Forceps02,
            Clamp01,     
            Clamp02,
        }
        
        [NTOutput] public bool result;

        public object GetValue() {
            return GetInputValue<object>("result");
        }

        public override string GetDisplayName(){
            return "Object is type of";
        }
    }
}
