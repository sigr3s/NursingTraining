using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTBool : NTVariable{
        public bool value;
        public bool defaultValue;

        public NTBool(){

        }

        public NTBool(string key, object defaultValue) : base(key, defaultValue)
        {
            this.variableName = key;
            this.value = (bool) defaultValue;
            this.defaultValue = (bool) defaultValue;
        }

        public override void Deserialize(){
            value = bool.Parse(serializedValue);
            defaultValue = bool.Parse(serializedDefaultValue);
        }

        public override void Serialize(){
            serializedValue = value.ToString();
            serializedDefaultValue = defaultValue.ToString();
        }

        public override void SetValue(object value){
            this.value = (bool) value;
        }

        public override void SetDefaultValue(object value){
            this.defaultValue = (bool) value;
        }

        public override object GetValue(){ 
            return value; 
        }

        public override void Reset(){
            value = defaultValue;
        }
    }
}