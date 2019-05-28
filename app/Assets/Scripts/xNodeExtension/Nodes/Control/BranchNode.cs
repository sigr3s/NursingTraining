using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes.Control{
    [System.Serializable]
    public class BranchNode : NTNode
    {
        [NTInput]  public DummyConnection flowIn;
        [NTOutput] public DummyConnection trueBranch;
        [NTOutput] public DummyConnection falseBranch;

        [NTInput] public bool condition;

        public override NodeExecutionContext NextNode(NodeExecutionContext context){

            Debug.Log("Next node from branch??");
            
            bool condition =  GetInputValue<bool>(nameof(condition), this.condition);

            string nemeOfPort = condition ? nameof(trueBranch) : nameof(falseBranch);

            Debug.Log(nemeOfPort);

            NTNode node = GetNode(nemeOfPort);
            NodePort port = GetPort(nemeOfPort);

            return new NodeExecutionContext{node = node, inputPort = port?.Connection, outputPort = port};
        }

        public override string GetDisplayName(){
            return "Branch";
        }

        
    }
}