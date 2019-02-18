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

        public bool AddVariable<T>(T value) where T: NTVariable{
            if(string.IsNullOrEmpty(value.GetKey())) return false;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary();

            if( dictionary.ContainsKey(newVariableType) ){
                variableTypeDictionary = dictionary[newVariableType];
                if(variableTypeDictionary.ContainsKey(value.GetKey())){    return false;   }
                else{
                    variableTypeDictionary.Add(value.GetKey(),value);
                    dictionary[newVariableType] = variableTypeDictionary;
                    return true;
                }
            }
            else{
                variableTypeDictionary.Add(value.GetKey(),value);
                dictionary.Add(newVariableType, variableTypeDictionary);
                return true;
            }
        }

        public bool AddVariable(NTVariable value, Type t){
            
            if(string.IsNullOrEmpty(value.GetKey())) return false;

            Type newVariableType = t;
            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary();

            if( dictionary.ContainsKey(newVariableType) ){
                variableTypeDictionary = dictionary[newVariableType];
                if(variableTypeDictionary.ContainsKey(value.GetKey())){ return false; }
                else{
                    variableTypeDictionary.Add(value.GetKey(),value);
                    dictionary[newVariableType] = variableTypeDictionary;
                    return true;
                }
            }
            else{
                variableTypeDictionary.Add(value.GetKey(),value);
                dictionary.Add(newVariableType, variableTypeDictionary);
                return true;
            }
        }

        public void RemoveVariable<T>(string key) where T: NTVariable{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            if( dictionary.ContainsKey(newVariableType) ){
                NTVariableDictionary variableTypeDictionary = dictionary[newVariableType];
                variableTypeDictionary.Remove(key);
                dictionary[newVariableType] = variableTypeDictionary;
            }
        }

        public T GetValue<T>(string key) where T: NTVariable{
            if(string.IsNullOrEmpty(key)) return default(T);
            
            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary();

            if( dictionary.ContainsKey(newVariableType) ){
                variableTypeDictionary = dictionary[newVariableType];
                if(variableTypeDictionary.ContainsKey(key)){

                    ///DOWNCAST!
                    var serializedParent = JsonUtility.ToJson( variableTypeDictionary[key]); 
                    T returnVar  = JsonUtility.FromJson<T>(serializedParent);

                    return returnVar;   
                }
            }

            return default(T);
        }

        public void SetValue<T>(string key, object value) where T: NTVariable{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary();

            if( dictionary.ContainsKey(newVariableType) ){
                variableTypeDictionary = dictionary[newVariableType];
                if(variableTypeDictionary.ContainsKey(key)){
                    
                    ///DOWNCAST!
                    var serializedParent = JsonUtility.ToJson( variableTypeDictionary[key]); 
                    T returnVar  = JsonUtility.FromJson<T>(serializedParent);

                    returnVar.SetValue(value);
                    returnVar.Serialize();

                    variableTypeDictionary[key] = returnVar;    
                    dictionary[newVariableType] = variableTypeDictionary;   
                }
            }

            return;
        }

        public void SetDefaultValue<T>(string key, object value) where T: NTVariable{
            if(string.IsNullOrEmpty(key)) return;

            Type newVariableType = typeof(T);
            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary();

            if( dictionary.ContainsKey(newVariableType) ){
                variableTypeDictionary = dictionary[newVariableType];
                if(variableTypeDictionary.ContainsKey(key)){
                    
                    var serializedParent = JsonUtility.ToJson( variableTypeDictionary[key]); 
                    T returnVar  = JsonUtility.FromJson<T>(serializedParent);

                    returnVar.SetDefaultValue(value);
                    returnVar.Serialize();

                    variableTypeDictionary[key] = returnVar;    
                    dictionary[newVariableType] = variableTypeDictionary;   
                }
            }

            return;
        }

        public List<string> GetOptions<T>(string key, out int index) where T: NTVariable
        {
            List<string> options = new List<string>();
            index = -1;

            Type newVariableType = typeof(T);

            NTVariableDictionary variableTypeDictionary = new NTVariableDictionary();

            if(dictionary.ContainsKey(newVariableType) ){
                variableTypeDictionary = dictionary[newVariableType];
                options = variableTypeDictionary.keys;
                index = options.IndexOf(key);
            }

            return options;
        }

        public void ResetToDefault(){
            foreach(NTVariableDictionary val in dictionary.values){
                foreach(NTVariable ntvar in val.values){
                    ntvar.Reset();
                }
            }
        }

    }

    [Serializable]
    public class NTTypedDictionary : Dictionary<Type, NTVariableDictionary>, ISerializationCallbackReceiver{
        [SerializeField] public List<string> keys = new List<string>();
        [SerializeField] public List<NTVariableDictionary> values = new List<NTVariableDictionary>();

        // save the dictionary to lists
        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var pair in this) {
                keys.Add(pair.Key.ToString());
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize() {
            this.Clear();

            Assembly asm = typeof(NTNode).Assembly;


            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++)
                this.Add( asm.GetType(keys[i]), values[i]);
        }
    }

    [Serializable]
    public class NTVariableDictionary : Dictionary<string, NTVariable>, ISerializationCallbackReceiver{
        [SerializeField] public List<string> keys = new List<string>();
        [SerializeField] public List<NTVariable> values = new List<NTVariable>();

        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var pair in this) {
                keys.Add(pair.Key);
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
}