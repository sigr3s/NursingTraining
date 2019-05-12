

using OdinSerializer;
using XNode;

namespace  NT.Graph
{
    public static class NodeGraphExtensions
    {
        public static NodeGraph Copy(this NodeGraph graph){
            byte[] copyData = SerializationUtility.SerializeValue(graph, DataFormat.Binary);
            NodeGraph gcopy = (NodeGraph) SerializationUtility.DeserializeValue<NodeGraph>(copyData, DataFormat.Binary);
            
            return gcopy;
        } 
    }
}