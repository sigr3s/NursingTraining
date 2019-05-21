using System;
using UnityEngine;

namespace NT.Variables
{
    [System.Serializable]
    public struct NTVariableData{
        [SerializeField] public string Name;
        [SerializeField] public string Value;
        [SerializeField] public string DefaultValue;
    }

    [Serializable]
    public class NTVariable<T> : NTVariable
    {
        public T value;
        public T defaultValue;

        public NTVariable(){
        }

        public override void SetValue(object value){
            this.value = (T) value;
        }
        public override void SetDefaultValue(object value){
            this.defaultValue = (T) value;
            this.value = this.defaultValue;
        }

        public override object GetValue(){
            return this.value;
        }

        public override object GetDefaultValue(){
            return this.defaultValue;
        }


        public override void Reset(){ 
            value = defaultValue;
        }

        public override Type GetDataType(){
            return typeof(T);
        }
    }

    [Serializable]
    public class NTVariable : INTVaribale
    {
        public bool collapsed = true;
        public string name;

        public virtual Type GetDataType(){ return null;}

        public virtual object GetDefaultValue(){ return null;}

        public string GetKey(){return name; }

        public virtual object GetValue(){return null;}

        public virtual bool IsCollapsed(){return collapsed;}

        public virtual void Reset(){}

        public virtual void SetCollapsed(bool collapsed){this.collapsed = collapsed;}

        public virtual void SetDefaultValue(object value){}

        public void SetKey(string key) {name = key;}

        public virtual void SetValue(object value){}

        public virtual NTVariableData ToNTVariableData(){return new NTVariableData();}

        public virtual bool Evaluate(Operator op, string value, bool isLeft){
            return true;
        }

        public virtual bool Evaluate(Operator op, NTVariable value){
            return true;
        }


    }

    public interface INTVaribale
    {
        void SetValue(object value);
        void SetDefaultValue(object value);
        object GetValue();
        object GetDefaultValue();

        void Reset();

        string GetKey();
        void SetKey(string key);

        Type GetDataType();

        bool IsCollapsed();
        void SetCollapsed(bool collapsed);
    }

    public enum Operator
    {
        Equals,
        LessThan,
        GreaterThan,
        NotEquals,
        LessOrEqualThan,
        GreaterOrEqualThan
    }
}