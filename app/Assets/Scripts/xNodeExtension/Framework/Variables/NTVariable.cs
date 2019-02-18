using System;
using UnityEngine;

namespace NT.Variables
{
    [Serializable]
    public class NTVariable : ISerializationCallbackReceiver
    {
        [SerializeField] public System.Type objectType;
        [SerializeField] public string variableName;

        [SerializeField] public string serializedValue;
        [SerializeField] public string serializedDefaultValue;

        public NTVariable(){}

        public void OnAfterDeserialize(){ 
            Deserialize();
        }

        public void OnBeforeSerialize(){ 
            Serialize();
        }

        public virtual void Deserialize(){}

        public virtual void Serialize(){}

        public NTVariable(string key, object defaultValue){}


        public virtual void SetValue(object value){ }

        public virtual void SetDefaultValue(object value){ }

        public virtual object GetValue(){ return null; }

        public virtual void Reset(){ }

        public string GetKey(){ return this.variableName; }

        public void SetKey(string key){ this.variableName = key; }

    }

    
}