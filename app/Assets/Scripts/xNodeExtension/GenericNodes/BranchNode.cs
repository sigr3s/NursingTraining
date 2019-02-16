using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class BranchNode : NTNode
    {
        [NTInput]  public DummyConnection flowIn;
        [NTOutput] public DummyConnection trueBranch;
        [NTOutput] public DummyConnection falseBranch;

        [NTInput] public bool condition;

        public override NodeExecutionContext NextNode(NodeExecutionContext context){
            
            bool condition =  GetInputValue<bool>(nameof(condition), this.condition);

            string nemeOfPort = condition ? nameof(trueBranch) : nameof(falseBranch);

            return new NodeExecutionContext( GetNode(nemeOfPort), GetPort(nemeOfPort) );
        }
    }
}