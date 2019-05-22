using System.Collections.Generic;
using NT.Atributes;
using UnityEngine;
using XNode;
using static NT.Nodes.Control.LogicNode;

namespace NT.Nodes.Control
{
    [System.Serializable]
    public class LogicOperationNode : NTNode{
        [NTOutput(ShowBackingValue.Never)] public bool result;

        public LogicOperation operation;

        [NTInput] public bool condittion0 = true;
        [NTInput] public bool condittion1 = true;


        [HideInInspector] public List<string> extraConditions;


        public override object GetValue(NodePort port){
            bool defaultValue = operation == LogicOperation.AND ? true : false;

            bool c0 = GetInputValue<bool>(nameof(condittion0), this.condittion0);
            bool c1 = GetInputValue<bool>(nameof(condittion1), this.condittion1);

            bool retValue = ApplyOperation(c0, c1, operation);

            if(extraConditions != null){
            
                foreach (string extraCondition in extraConditions)
                {
                    bool cE = GetInputValue<bool>(nameof(extraCondition), retValue);

                    retValue = ApplyOperation(retValue, cE, operation);               
                }
            }
            
            return retValue;
        }

        bool ApplyOperation(bool b0, bool b1, LogicOperation operation){
            switch(operation){
                case LogicOperation.AND:
                    return (b0 && b1);
                default:
                    return (b0 || b1);
            }
        }

        public override string GetDisplayName(){
            return "Boolean Operation";
        }

        
        public enum LogicOperation{
            AND,
            OR
        }

    }
    
}