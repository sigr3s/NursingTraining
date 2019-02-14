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

        public override NTNode NextNode(){
            bool condition =  GetInputValue<bool>(nameof(condition), this.condition);

            NTNode nextNode = GetNode(condition ? nameof(trueBranch) : nameof(falseBranch));
            
            return nextNode;
        }
    }
}