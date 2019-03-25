using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTFloat : NTVariable<float>{
       

        public override float DeserializeValue(string data){ 
            float v = 0;
            float.TryParse(data, out v);
            return v; 
        }

        public override string SerializeValue(float val){ return val.ToString(); }

        public override bool Evaluate(Operator op, string value, bool isLeft){

            float floatValue = 0f;

            if(!float.TryParse(value, out floatValue)){
                floatValue = 0;
            }

            return InternalEvaluate(op, floatValue, isLeft);
            
        }

        public override bool Evaluate(Operator op, NTVariable value){
            Type rightVariableType = value.GetDataType();

            if(rightVariableType == typeof(float) || rightVariableType == typeof(int) || rightVariableType == typeof(double)){
                float rightValue = (float) value.GetValue(); 
                return InternalEvaluate(op, rightValue, true);
            }
            else{
                return this.value > 0;
            }
            
        }

        private bool InternalEvaluate(Operator op, float value, bool isLeft){
            switch(op){
                case Operator.Equals:
                    return (this.value == value);
                case Operator.NotEquals:
                    return (this.value != value);
                case Operator.GreaterThan:
                    if(isLeft) return  this.value > value;
                    else return  value > this.value;                    
                case Operator.LessThan:
                    if(isLeft) return  this.value < value;
                    else return  value < this.value; 
                case Operator.GreaterOrEqualThan:
                    if(isLeft) return  this.value >= value;
                    else return  value >= this.value; 
                case Operator.LessOrEqualThan:
                    if(isLeft) return  this.value <= value;
                    else return  value <= this.value; 
                default:
                    return false;
            }
        }
    
    }
}