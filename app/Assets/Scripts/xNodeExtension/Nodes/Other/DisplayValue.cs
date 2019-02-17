
namespace NT.Nodes.Other {
    
    public class DisplayValue : FlowNode {

        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DummyConnection input;
        

        public object GetValue() {
            return GetInputValue<object>("input");
        }
    }
}
