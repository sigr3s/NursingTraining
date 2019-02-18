using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTString : NTVariable{
        public string value;
        public string defaultValue;

        public NTString(){

        }

        public NTString(string key, object defaultValue) : base(key, defaultValue)
        {
            this.variableName = key;
            this.value = (string) defaultValue;
            this.defaultValue = (string) defaultValue;
        }

        public override void Deserialize(){
            value = serializedValue;
            defaultValue = serializedDefaultValue;
        }

        public override void Serialize(){
            serializedValue = value;
            serializedDefaultValue = defaultValue;
        }

        public override void SetValue(object value){
            this.value = (string) value;
        }

        public override void SetDefaultValue(object value){
            this.defaultValue = (string) value;
        }

        public override object GetValue(){ 
            return value; 
        }

        public override void Reset(){
            value = defaultValue;
        }


    }
}