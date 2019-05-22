using System;
using System.Collections.Generic;
using NT.Atributes;
using NT.Variables;
using UnityEngine;
using XNode;

namespace NT.Nodes.Control
{
    [System.Serializable]
    public class LogicNode : NTNode{

        [NTInput(typeConstraint = TypeConstraint.None)] public ValueConnection input0;

        public Operator op;

        [NTInput(typeConstraint = TypeConstraint.None)] public ValueConnection input1;




        [NTOutput(ShowBackingValue.Never)] public bool result;


        public override object GetValue(NodePort port){
            object val0 =  GetInputValue<object>(nameof(input0), null);
            object val1 =  GetInputValue<object>(nameof(input1), null);

            NodePort portVal0 = GetPort(nameof(input0));
            NodePort portVal1 = GetPort(nameof(input1));

            hasError = false;
            error = "";

            //Both Ports connected
            if(portVal0.IsConnected && portVal1.IsConnected){
                if(val0 == null || val1 == null){
                    error = "Values cannot be null";
                    hasError = true;
                    return false;
                }

                Type val0Type = val0.GetType();
                Type val1Type = val1.GetType();

                if(val0Type.IsNumber()){
                    if(val1Type.IsNumber()){
                        return CompareNumbers(val0, val1, op);
                    }
                    else
                    {
                        error = "Input types are not compatible";
                        hasError = true;
                        return false;
                    }
                }
                else if(val0Type.IsString())
                {
                    if(val1Type.IsString()){
                        return CompareString(val0, val1, op);
                    }
                    else
                    {
                        error = "Input types are not compatible";
                        hasError = true;
                        return false;
                    }
                }
                else if(val0Type.IsBool()){
                    if(val1Type.IsBool()){
                        return CompareBools(val0, val1, op);
                    }
                    else
                    {
                        error = "Input types are not compatible";
                        hasError = true;
                        return false;
                    }
                }
                else
                {
                    error = "Input types are not supported";
                    hasError = true;
                    return false;
                }
            }
            //Port 0 connected port 1 as string input
            else if(portVal0.IsConnected && !portVal1.IsConnected){
                if(val0 == null) return false;

                Type val0Type = val0.GetType();

                if(val0 == null){
                    error = "Values cannot be null";
                    hasError = true;
                    return false;
                }

                if(val0Type.IsNumber()){
                    float v1f = 0;
                    if(!float.TryParse(input1.value, out v1f)){
                        error = "Cannot parse string to number";
                        hasError = true;
                        return false;
                    }

                    return CompareNumbers(val0, v1f, op);
                }
                else if(val0Type.IsString())
                {
                    return CompareString(val0, input1.value, op);
                }
                else if(val0Type.IsBool())
                {
                    bool v1b = false;
                    if(!bool.TryParse(input1.value, out v1b)){
                        error = "Cannot parse string to bool";
                        hasError = true;
                        return false;
                    }

                    return CompareBools(val0, v1b, op);                    
                }
                else
                {
                    error = "Input types are not supported";
                    hasError = true;
                    return false;
                }

            }
            //Port 1 connected port 0 as string input
            else if(!portVal0.IsConnected && portVal1.IsConnected){
                Type val1Type = val1.GetType();

                if(val1 == null){
                    error = "Values cannot be null";
                    hasError = true;
                    return false;
                }

                if(val1Type.IsNumber()){
                    float v0f = 0;
                    if(!float.TryParse(input0.value, out v0f)){
                        error = "Cannot parse string to number";
                        hasError = true;
                        return false;
                    }

                    return CompareNumbers(v0f, val1, op);
                }
                else if(val1Type.IsString())
                {
                    return CompareString(input0.value, val1, op);
                }
                else if(val1Type.IsBool())
                {
                    bool v0b = false;
                    if(!bool.TryParse(input0.value, out v0b)){
                        error = "Cannot parse string to bool";
                        hasError = true;
                        return false;
                    }

                    return CompareBools(v0b, val1, op);                    
                }
                else
                {
                    error = "Input types are not supported";
                    hasError = true;
                    return false;
                }
            }
            //Both ports as string
            else{
                bool b0 = false;
                bool b1 = false;

                float f0 = 0;
                float f1 = 0;
                
                if(float.TryParse(input0.value, out f0)){
                    if(!float.TryParse(input1.value, out f1)){
                        error = "Input types are not compatible";
                        hasError = true;
                        return false;
                    }

                    return CompareNumbers(f0, f1, op);                    
                }
                else if(bool.TryParse(input0.value, out b0)){
                    if(!bool.TryParse(input1.value, out b1)){
                        error = "Input types are not compatible";
                        hasError = true;
                        return false;
                    }

                    return CompareBools(b0, b1, op);
                }
                else{
                    if(bool.TryParse(input1.value, out b1) || float.TryParse(input1.value, out f1)){
                        error = "Input types are not compatible";
                        hasError = true;
                        return false;
                    }

                    return CompareString(input0.value, input1.value, op);
                }
            }
        }

        public bool CompareNumbers(object val0, object val1, Operator oper){
            float v0 = Convert.ToSingle(val0);
            float v1 = Convert.ToSingle(val1);

            switch(oper){
                case Operator.Equals:
                    return v0 == v1;
                case Operator.NotEquals:
                    return v0 != v1;
                default:
                    hasError = true;
                    return false;
            }
        }

        public bool CompareString(object val0, object val1, Operator oper){
            string v0 = Convert.ToString(val0);
            string v1 = Convert.ToString(val1);

            switch(oper){
                case Operator.Equals:
                    return v0 == v1;
                case Operator.NotEquals:
                    return v0 != v1;
                default:
                    hasError = true;
                    return false;
            }
        }

        public bool CompareBools(object val0, object val1, Operator oper){
            bool v0 = Convert.ToBoolean(val0);
            bool v1 = Convert.ToBoolean(val1);

            switch(oper){
                case Operator.Equals:
                    return v0 == v1;
                case Operator.NotEquals:
                    return v0 != v1;
                default:
                    hasError = true;
                    return false;
            }
        }
    
        public override string GetDisplayName(){
            return "Compare";
        }
    }

    public enum LogicNodeTypes{
        lnumber,
        lstring
    }   
}