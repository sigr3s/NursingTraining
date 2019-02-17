
namespace NT.Nodes.Other {
    
    public class DisplayValue : NTNode {

        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DummyConnection input;
        

        public object GetValue() {
            return GetInputValue<object>("input");
        }
    }
}
