using System;
using UnityEngine;

namespace NT.Variables
{
    public class GenericVariable<T> {
        public string variableName;
        public static Color color;

        public T value;
        public T defaultValue;

        public GenericVariable(){

        }
        
        public GenericVariable(string key, T value){
            SetKey(key);
            SetValue(value);
            SetDefaultValue(value);
        }

        public void SetValue(T value){
            this.value = value;
        }

        public void SetDefaultValue(T value){
            this.defaultValue = value;
        }

        public T GetValue(){
            return value;
        }

        public void Reset(){
            this.value = this.defaultValue;
        }

        public string GetKey(){
            return this.variableName;
        }

        public void SetKey(string key){
            this.variableName = key;
        }

        public Type GetVariableType(){
            return typeof(T);
        }
    }
}