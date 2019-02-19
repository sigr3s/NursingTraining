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
    public abstract class NTVariable<T> : ISerializationCallbackReceiver, INTVaribale
    {
        public T value;
        public T defaultValue;
        public NTVariableData serializedData;
        public NTVariable(){}

        public void OnAfterDeserialize(){ 
            DeserializeValue(serializedData.Value);
            DeserializeDefaultValue(serializedData.Value);
        }

        public void OnBeforeSerialize(){ 
           serializedData.Value =  SerializeValue();
           serializedData.DefaultValue = SerializeDefaultValue();
        }

        public virtual void DeserializeValue(string data){ value = JsonUtility.FromJson<T>(data); }
        public virtual void DeserializeDefaultValue(string data){ defaultValue = JsonUtility.FromJson<T>(data);}

        public virtual string SerializeValue(){ return JsonUtility.ToJson(value);}
        public virtual string SerializeDefaultValue(){ return JsonUtility.ToJson(defaultValue);}


        public virtual void SetValue(object value){
            this.value = (T) value;
        }
        public virtual void SetDefaultValue(object value){
            this.defaultValue = (T) value;
        }

        public virtual object GetValue(){
            return this.value;
        }        
        public virtual object GetDefaultValue(){
            return this.defaultValue;
        }


        public virtual void Reset(){ 
            value = defaultValue;
        }

        public string GetKey(){ return this.serializedData.Name; }

        public void SetKey(string key){ this.serializedData.Name = key; }


        public void FromNTVariableData(NTVariableData data){
            this.serializedData = data;
            OnAfterDeserialize();
        }


        public NTVariableData ToNTVariableData(){
            OnBeforeSerialize();
            return this.serializedData;
        }

    }

    public interface INTVaribale
    {
        void SetValue(object value);
        void SetDefaultValue(object value);
        object GetValue();
        object GetDefaultValue();

        void Reset();

        void FromNTVariableData(NTVariableData data);
        NTVariableData ToNTVariableData();

        string GetKey();
        void SetKey(string key);
        
    }
}