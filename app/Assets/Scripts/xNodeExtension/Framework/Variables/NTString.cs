using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTString : NTVariable<string>{

        public override object GetValue(){
            if(this.value == null) return "";
            return base.GetValue();
        }

        public override bool Evaluate(Operator op, string value, bool isLeft){
            switch(op){
                case Operator.Equals:
                    return this.value == value;
                case Operator.NotEquals:
                    return this.value != value;
                default:
                    if(isLeft)
                        return !string.IsNullOrEmpty(this.value);
                    else
                        return !string.IsNullOrEmpty(value);
            }
        }

        public override bool Evaluate(Operator op, NTVariable value){
            Type rightVariableType = value.GetDataType();

            if(rightVariableType == typeof(string)){
                string rightValue = (string) value.GetValue(); 
                return Evaluate(op, rightValue, true);
            }
            else{
                return !string.IsNullOrEmpty(this.value);
            }
            
        }
    }
}