using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTInt : NTVariable{
        public int value;
        public int defaultValue;

        public NTInt(){

        }

        public NTInt(string key, object defaultValue) : base(key, defaultValue)
        {
            this.variableName = key;
            this.value = (int) defaultValue;
            this.defaultValue = (int) defaultValue;
        }

        public override void Deserialize(){
            value = int.Parse(serializedValue);
            defaultValue = int.Parse(serializedDefaultValue);
        }

        public override void Serialize(){
            serializedValue = value.ToString();
            serializedDefaultValue = defaultValue.ToString();
        }

        public override void SetValue(object value){
            this.value = (int) value;
        }

        public override void SetDefaultValue(object value){
            this.defaultValue = (int) value;
        }

        public override object GetValue(){ 
            return value; 
        }

        public override void Reset(){
            value = defaultValue;
        }
    }
}