using System;
using System.Collections.Generic;
using NT.Atributes;
using NT.Variables;
using UnityEngine;
using XNode;

namespace NT.Nodes.Control{
    [System.Serializable]
    public class LogicNode : NTNode
    {
        [NTOutput(ShowBackingValue.Never)] public bool result;
        public LogicOperation operation;

        [SerializeField] [HideInInspector] public List<LogicCondition> conditions;


        public enum LogicOperation
        {
            AND,
            OR
        }


        public override object GetValue(NodePort port){
            bool retValue = true;

            foreach(var condition in conditions){
                bool condValue = condition.Evaluate();

                switch(operation){
                    case LogicOperation.AND:
                        retValue = retValue && condValue;
                        if(!retValue) return retValue;
                    break;
                    default:
                        retValue = retValue || condValue;
                    break;
                }
            }

            return retValue;
        }
    }


    [System.Serializable]
    public class LogicCondition{
        public VariableOrInmediate leftSide;

        public Operator op;

        public VariableOrInmediate rightSide;



        public enum Operator
        {
            Equals,
            LessThan,
            GreaterThan,
            NotEquals,
            LessOrEqualThan,
            GreaterOrEqualThan
        }

        public bool Evaluate(){
            if(!leftSide.isVariable && !rightSide.isVariable){
                string lli = leftSide.value.ToLowerInvariant();
                string rri = rightSide.value.ToLowerInvariant();

                bool llib = false;
                bool rrib = false;

                if(bool.TryParse(lli, out llib)){

                    if(bool.TryParse(rri,out rrib)){
                        //Check for operation
                        switch(op){
                            case Operator.Equals:
                                return (rrib == llib);
                            case Operator.NotEquals:
                                return (rrib != llib);
                            default:
                                return (rrib || llib);
                        }
                    }
                    else
                    {
                        //only llib => then always return bool
                        return llib;
                    }

                }
                else if(bool.TryParse(rri, out rrib))
                {
                    //only rrib bool => then always return bool
                    return rrib;
                }

                float llif = 0;
                float rrif = 0;

                //Try parse values to float
                if(float.TryParse(lli, out llif)){
                    if(float.TryParse(rri, out rrif)){
                        switch(op){
                            case Operator.Equals:
                                return (llif == rrif);
                            case Operator.NotEquals:
                                return (llif != rrif);
                            case Operator.GreaterThan:
                                return  llif > rrif;
                            case Operator.LessThan:
                                return  llif < rrif;
                            case Operator.GreaterOrEqualThan:
                                return llif >= rrif;
                            case Operator.LessOrEqualThan:
                                return llif <= rrif;
                            default:
                                return false;
                        }
                    }
                    else
                    {
                        return llif > 0;
                    }
                }
                else if(float.TryParse(rri, out rrif))
                {
                    //only rrif is float
                    return rrif > 0;
                }

                //Both are strings
                switch(op){
                    case Operator.Equals:
                        return (lli == rri);
                    case Operator.NotEquals:
                        return (lli != rri);
                    default:
                        return !string.IsNullOrEmpty(lli);
                }
            }
            //Left is variable
            else if(leftSide.isVariable && !rightSide.isVariable){
                
            }
            //Right is varaiables
            else if(!leftSide.isVariable && rightSide.isVariable){

            }
            //Both are varaiables
            else
            {

            }
            return false;
        }
    }

    [System.Serializable]
    public struct VariableOrInmediate{
        public string value;
        public string variableName;
        public string variableType;
        public Type VariableType;

        public bool isVariable;
    }
}