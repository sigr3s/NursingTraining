using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace NT.Variables
{
    [Serializable]
    public class NTVariableRepository {
        [SerializeField] public NTTypedDictionary dictionary;
        [HideInInspector] public UnityEvent onModified;

        public NTVariableRepository(){

            dictionary = new NTTypedDictionary();
        }

        public bool AddVariable<T>(T value) where T: NTVariable{
            if(string.IsNullOrEmpty(value.GetKey())) return false;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary(newVariableType);

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(value.GetKey())){    return false;   }
                else{
                    variableTypeDictionary.Add(value.GetKey(),value);
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;
                    onModified?.Invoke();
                    return true;
                }
            }
            else{
                variableTypeDictionary.Add(value.GetKey(),value);
                dictionary.Add(newVariableType.ToString(), variableTypeDictionary);
                onModified?.Invoke();
                return true;
            }
        }

         public bool AddVariable(Type varType, object value){
            NTVariable ntVar = (NTVariable) value;
            if(ntVar == null) return false;
            if(string.IsNullOrEmpty(ntVar.GetKey())) return false;

            Type newVariableType = varType;
            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary(newVariableType);

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(ntVar.GetKey())){    return false;   }
                else{
                    variableTypeDictionary.Add(ntVar.GetKey(),ntVar);
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;
                    onModified?.Invoke();
                    return true;
                }
            }
            else{
                variableTypeDictionary.Add(ntVar.GetKey(),ntVar);
                dictionary.Add(newVariableType.ToString(), variableTypeDictionary);                
                onModified?.Invoke();
                return true;
            }
        }


        public bool AddVariable(NTVariable value, Type t){
            
            if(string.IsNullOrEmpty(value.GetKey())) return false;

            Type newVariableType = t;

            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary(t);

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(value.GetKey())){ return false; }
                else{
                    variableTypeDictionary.Add(value.GetKey(),value);
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;
                    onModified?.Invoke();
                    return true;
                }
            }
            else{
                variableTypeDictionary.Add(value.GetKey(),value);
                dictionary.Add(newVariableType.ToString(), variableTypeDictionary);                
                onModified?.Invoke();
                return true;
            }
        }

        public void RemoveVariable<T>(string key) where T: NTVariable{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                NTVariableDictionary variableTypeDictionary = dictionary[newVariableType.ToString()];
                variableTypeDictionary.Remove(key);
                dictionary[newVariableType.ToString()] = variableTypeDictionary;
                onModified?.Invoke();
            }
        }

        public object GetValue<T>(string key) where T: NTVariable{
            if(string.IsNullOrEmpty(key)) return default(object);

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){

                    NTVariable ntvar = variableTypeDictionary[key];
                    return ntvar.GetValue();
                }

                Debug.Log("No value???"  + key + "   " + variableTypeDictionary.DictType);
            }


            return default(object);
        }

        public object GetValue(string key, Type t){
            if(string.IsNullOrEmpty(key)) return default(object);

            Type newVariableType = t;
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){

                    NTVariable ntvar = variableTypeDictionary[key];
                    return ntvar.GetValue();
                }
            }

            return default(object);
        }

        public object GetNTValue(string key, Type t){
            if(string.IsNullOrEmpty(key)) return default(object);

            Type newVariableType = t;
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){

                    NTVariable ntvar = variableTypeDictionary[key];
                    return ntvar;
                }
            }

            return default(object);
        }

        public void SetValue<T>(string key, object value) where T: NTVariable{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){
                    NTVariable ntvar = variableTypeDictionary[key];
                    ntvar.SetValue(value);

                    variableTypeDictionary[key] = ntvar;
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;
                }
            }

            return;
        }

        public void SetValue(Type ntVariableType, string key, object value){
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = ntVariableType;
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){
                    NTVariable ntvar = variableTypeDictionary[key];
                    ntvar.SetValue(value);

                    variableTypeDictionary[key] = ntvar;
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;
                }
            }

            return;
        }

        public void SetDefaultValue<T>(string key, object value) where T: NTVariable{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){
                    NTVariable ntvar = variableTypeDictionary[key];
                    ntvar.SetDefaultValue(value);

                    variableTypeDictionary[key] = ntvar;
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;
                }
            }

            return;
        }

        public List<string> GetOptions(Type variableType, string key, out int index)
        {
            List<string> options = new List<string>();
            index = -1;

            Type newVariableType = variableType;

            if(newVariableType == null) return options;

            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary(newVariableType);

            if(dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                options = variableTypeDictionary.keys;
                index = options.IndexOf(key);
            }

            return options;
        }

        public void ResetToDefault(){
            foreach(NTVariableDictionary val in dictionary.values){
                foreach(KeyValuePair<string, NTVariable> entry in val)
                {
                    entry.Value.Reset();
                }
            }
        }

    }

    [Serializable]
    public class NTTypedDictionary : Dictionary<string, NTVariableDictionary>, ISerializationCallbackReceiver{
        [SerializeField] public List<string> keys = new List<string>();
        [SerializeField] public List<NTVariableDictionary> values = new List<NTVariableDictionary>();

        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var pair in this) {
                keys.Add(pair.Key.ToString());
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize() {
            this.Clear();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++){
                values[i].OnAfterDeserialize();
                this.Add(keys[i], values[i]);
            }
        }
    }

    [Serializable]
    public class NTVariableDictionary : Dictionary<string, NTVariable>, ISerializationCallbackReceiver{
        [SerializeField] public List<string> keys = new List<string>();
        [SerializeField] public List<NTVariableData> values = new List<NTVariableData>();
        [SerializeField] public string DictType;

        public Type _dictType { get; private set; }
        public NTVariableDictionary(){
            DictType = typeof(object).AssemblyQualifiedName;
            _dictType = typeof(object);
        }

        public NTVariableDictionary(Type t){
            DictType = t.AssemblyQualifiedName;
            _dictType = t;
        }

        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var pair in this) {
                keys.Add(pair.Key);
                values.Add(pair.Value.ToNTVariableData());
            }
        }

        public void OnAfterDeserialize() {
            this.Clear();
            Type t = Type.GetType(DictType);

            _dictType = t;        
            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++){
                NTVariable instance = (NTVariable)FormatterServices.GetUninitializedObject(t); //does not call ctor
                instance.FromNTVariableData(values[i]);
                this.Add(keys[i], instance);
            }
        }
    }
}