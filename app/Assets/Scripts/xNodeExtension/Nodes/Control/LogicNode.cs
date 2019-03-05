using System;
using System.Collections.Generic;
using NT.Atributes;
using NT.Graph;
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
            bool retValue = operation == LogicOperation.AND ? true : false;


            foreach(var condition in conditions){
                bool condValue = condition.Evaluate((NTGraph) graph);

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


        public bool Evaluate(NTGraph graph){
            if(!leftSide.isVariable && !rightSide.isVariable){

                string lli = leftSide.value;
                string rri = rightSide.value;

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
                NTVariable vleftVar = (NTVariable) graph.sceneVariables.variableRepository.GetNTValue(leftSide.variableName, leftSide.VariableType);

                if(vleftVar != null){
                    return vleftVar.Evaluate(op, rightSide.value, true);
                }
                else
                {
                    return false;
                }
            }
            //Right is varaiables
            else if(!leftSide.isVariable && rightSide.isVariable){
                NTVariable vrightVar = (NTVariable) graph.sceneVariables.variableRepository.GetNTValue(rightSide.variableName, rightSide.VariableType);

                if(vrightVar != null){
                    return vrightVar.Evaluate(op, rightSide.value, false);
                }
                else
                {
                    return false;
                }
            }
            //Both are varaiables
            else
            {               
                NTVariable vleftVar = (NTVariable) graph.sceneVariables.variableRepository.GetNTValue(leftSide.variableName, leftSide.VariableType);
                NTVariable vrightVar = (NTVariable) graph.sceneVariables.variableRepository.GetNTValue(leftSide.variableName, leftSide.VariableType);

                if(vleftVar != null){
                    return vleftVar.Evaluate(op, vrightVar);
                }
                else if(vrightVar != null)
                {
                    return vrightVar.Evaluate(op, vleftVar);
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public struct VariableOrInmediate{
        public string value;
        public string variableName;
        public string variableType;

        public Type _variableType;
        public Type VariableType{
            get{
                if(string.IsNullOrEmpty(variableType)) return null;

                if(_variableType == null || _variableType.AssemblyQualifiedName != variableType){
                    _variableType = Type.GetType(variableType);
                }

                return _variableType;
            }
        }

        public bool isVariable;
    }
}