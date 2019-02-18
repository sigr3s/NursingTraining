using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTFloat : NTVariable{
        public float value;
        public float defaultValue;

        public NTFloat(){

        }
        
        public NTFloat(string key, object defaultValue) : base(key, defaultValue)
        {
            this.variableName = key;
            this.value = (float) defaultValue;
            this.defaultValue = (float) defaultValue;
        }

        public override void Deserialize(){
            value = float.Parse(serializedValue);
            defaultValue = float.Parse(serializedDefaultValue);
        }

        public override void Serialize(){
            serializedValue = value.ToString();
            serializedDefaultValue = defaultValue.ToString();
        }

        public override void SetValue(object value){
            this.value = (float) value;
        }

        public override void SetDefaultValue(object value){
            this.defaultValue = (float) value;
        }

        public override object GetValue(){ 
            return value; 
        }

        public override void Reset(){
            value = defaultValue;
        }
    }
}