using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes.Messages{
    [System.Serializable]
    public class CallbackNode : NTNode
    {
        [NTOutput] public DummyConnection flowOut;

        [SerializeField] public string key;
        [SerializeField] [HideInInspector] public string linkedToSceneObject;


        public override object GetValue(NodePort port) {
            return null;
        }

        public override NodeExecutionContext NextNode(NodeExecutionContext context){
            NTNode node = GetNode(nameof(flowOut));
            NodePort port = GetPort(nameof(flowOut));

            return new NodeExecutionContext{node = node, inputPort = port?.Connection, outputPort = port};
        }

        public string GetCallbackKey(){
            return linkedToSceneObject + key;
        }

        public override string GetDisplayName(){
            return "Message Recieved";
        }

        public override int GetWidth(){
            return 250;
        }
    }
}