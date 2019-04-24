using NT.Atributes;

namespace NT.Nodes.Other {
    
    public class CompareTypes : NTNode {

        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DummyConnection type01;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DummyConnection type02;
        
        [NTOutput] public bool result;

        public object GetValue() {
            return GetInputValue<object>("result");
        }
    }
}
