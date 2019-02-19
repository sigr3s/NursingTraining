using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NT.Variables
{
    [Serializable]
    public class NTVariableRepository {
        [SerializeField] public NTTypedDictionary dictionary;

        public NTVariableRepository(){

            dictionary = new NTTypedDictionary();
        }

        public bool AddVariable<T>(T value) where T: INTVaribale{
            if(string.IsNullOrEmpty(value.GetKey())) return false;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary(newVariableType);

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(value.GetKey())){    return false;   }
                else{
                    variableTypeDictionary.Add(value.GetKey(),value);
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;
                    return true;
                }
            }
            else{
                variableTypeDictionary.Add(value.GetKey(),value);
                dictionary.Add(newVariableType.ToString(), variableTypeDictionary);
                return true;
            }
        }

        public bool AddVariable(INTVaribale value, Type t){
            
            if(string.IsNullOrEmpty(value.GetKey())) return false;

            Type newVariableType = t;

            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary(t);

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(value.GetKey())){ return false; }
                else{
                    variableTypeDictionary.Add(value.GetKey(),value);
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;
                    return true;
                }
            }
            else{
                variableTypeDictionary.Add(value.GetKey(),value);
                dictionary.Add(newVariableType.ToString(), variableTypeDictionary);
                return true;
            }
        }

        public void RemoveVariable<T>(string key) where T: INTVaribale{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                NTVariableDictionary variableTypeDictionary = dictionary[newVariableType.ToString()];
                variableTypeDictionary.Remove(key);
                dictionary[newVariableType.ToString()] = variableTypeDictionary;
            }
        }

        public object GetValue<T>(string key) where T: INTVaribale{
            if(string.IsNullOrEmpty(key)) return default(object);
            
            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){

                    INTVaribale ntvar = variableTypeDictionary[key];
                    return ntvar.GetValue();   
                }
                
                Debug.Log("No value???"  + key + "   " + variableTypeDictionary.DictType);
            }


            return default(object);
        }

        public void SetValue<T>(string key, object value) where T: INTVaribale{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){
                    INTVaribale ntvar = variableTypeDictionary[key];
                    ntvar.SetValue(value);

                    variableTypeDictionary[key] = ntvar;    
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;   
                }
            }

            return;
        }

        public void SetDefaultValue<T>(string key, object value) where T: INTVaribale{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary;

            if( dictionary.ContainsKey(newVariableType.ToString()) ){
                variableTypeDictionary = dictionary[newVariableType.ToString()];
                if(variableTypeDictionary.ContainsKey(key)){
                    INTVaribale ntvar = variableTypeDictionary[key];
                    ntvar.SetDefaultValue(value);

                    variableTypeDictionary[key] = ntvar;    
                    dictionary[newVariableType.ToString()] = variableTypeDictionary;   
                }
            }

            return;
        }

        public List<string> GetOptions<T>(string key, out int index) where T: INTVaribale
        {
            List<string> options = new List<string>();
            index = -1;

            Type newVariableType = typeof(T);

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
                foreach(KeyValuePair<string, INTVaribale> entry in val)
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

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }

    [Serializable]
    public class NTVariableDictionary : Dictionary<string, INTVaribale>, ISerializationCallbackReceiver{
        [SerializeField] public List<string> keys = new List<string>();
        [SerializeField] public List<NTVariableData> values = new List<NTVariableData>();
        [SerializeField] public string DictType;

        private NTVariableDictionary(){
            DictType = typeof(object).AssemblyQualifiedName;
        }

        public NTVariableDictionary(Type t){
            DictType = t.AssemblyQualifiedName;
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
            
            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++){
                INTVaribale instance = (INTVaribale) Activator.CreateInstance(t);
                instance.FromNTVariableData(values[i]);
                this.Add(keys[i], instance);
            }
        }
    }
}