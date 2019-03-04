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
    public class NTVariable<T> : NTVariable, ISerializationCallbackReceiver
    {
        public T value;
        public T defaultValue;

        public NTVariable(){
        }

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


        public override void SetValue(object value){
            Debug.Log("set value?");
            this.value = (T) value;
        }
        public override void SetDefaultValue(object value){
            this.defaultValue = (T) value;
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


        public override void FromNTVariableData(NTVariableData data){
            this.serializedData = data;
            OnAfterDeserialize();
            SetKey(data.Name);
        }


        public override NTVariableData ToNTVariableData(){
            OnBeforeSerialize();
            return this.serializedData;
        }

        public override Type GetDataType(){
            return typeof(T);
        }
    }

    [Serializable]
    public class NTVariable : INTVaribale
    {
        public NTVariableData serializedData;
        public bool collapsed = true;

        public virtual void FromNTVariableData(NTVariableData data){}

        public virtual Type GetDataType(){ return null;}

        public virtual object GetDefaultValue(){ return null;}

        public string GetKey(){return serializedData.Name; }

        public virtual object GetValue(){return null;}

        public virtual bool IsCollapsed(){return collapsed;}

        public virtual void Reset(){}

        public virtual void SetCaollapsed(bool collapsed){this.collapsed = collapsed;}

        public virtual void SetDefaultValue(object value){}

        public void SetKey(string key) {this.serializedData.Name = key;}

        public virtual void SetValue(object value){}

        public virtual NTVariableData ToNTVariableData(){return new NTVariableData();}
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

        Type GetDataType();

        bool IsCollapsed();
        void SetCaollapsed(bool collapsed);
    }
}