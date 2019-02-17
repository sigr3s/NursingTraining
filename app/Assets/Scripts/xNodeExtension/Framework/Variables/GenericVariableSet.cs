using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.Variables
{
    [Serializable]
    public class GenericVariables<T,K> where T : GenericVariable<K>, new(){
        [SerializeField] private List<T> values;
        [SerializeField] private List<string> keys;

        public GenericVariables(){
            values = new List<T>();
            keys = new List<string>();
        }

        public bool AddVariable(string key, K value){
            int index = keys.IndexOf(key);
            if(index >= 0){
                return false;
            }
            else
            {
                T v = new T();
                v.SetKey(key);
                v.SetValue(value);
                v.SetDefaultValue(value);

                values.Add(v);
                keys.Add(key);

                return true;   
            }
        }

        public void RemoveVariable(string key){
            int index = keys.IndexOf(key);
            if(index >= 0){
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
        }

        public K GetValue(string key){
            int index = keys.IndexOf(key);
            if(index >= 0){
                return values[index].value;
            }
            else
            {
                return GetDefaultValue();
            }
        }

        public void SetValue(string key, K value){
            int index = keys.IndexOf(key);
            if(index >= 0){
                values[index].SetValue(value);
            }
        }

        public void SetDefaultValue(string key, K defaultValue){
            int index = keys.IndexOf(key);
            if(index >= 0){
                values[index].SetDefaultValue(defaultValue);
            }
        }

        public List<string> GetOptions(string key, out int index)
        {
            index = keys.IndexOf(key);
            return keys;
        }

        public virtual K GetDefaultValue(){
            return default(K);
        }

        public void ResetToDefault(){
            foreach(T val in values){
                val.Reset();
            }
        }

    }    
}