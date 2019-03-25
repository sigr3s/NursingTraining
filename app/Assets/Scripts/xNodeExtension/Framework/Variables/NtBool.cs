using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTBool : NTVariable<bool>{

        public override bool DeserializeValue(string data){ 
            bool b = false;
            bool.TryParse(data, out b);
            return b; 
        }

        public override string SerializeValue(bool val){
            return val.ToString(); 
        }


        public override bool Evaluate(Operator op, string value, bool isLeft){

            bool conditionBool = false;

            if(!bool.TryParse(value, out conditionBool)){
                conditionBool = !string.IsNullOrEmpty(value);
            }

            return InternalEvaluate(op, conditionBool, isLeft);
            
        }

        public override bool Evaluate(Operator op, NTVariable value){
            Type rightVariableType = value.GetDataType();

            if(rightVariableType == typeof(bool)){
                bool rightValue = (bool) value.GetValue(); 
                return InternalEvaluate(op, rightValue, true);
            }
            else{
                return this.value;
            }
            
        }

        private bool InternalEvaluate(Operator op, bool value, bool isLeft){
            switch(op){
                case Operator.Equals:
                    return this.value == value;
                case Operator.NotEquals:
                    return this.value != value;
                default:
                    if(isLeft)
                        return this.value;
                    else
                        return value;
            }
        }
    }
}