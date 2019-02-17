using System;
using System.Collections.Generic;
using NT.Graph;
using UnityEngine;


namespace NT.Variables
{
    [CreateAssetMenu(fileName = "SceneVariables", menuName = "NT/SceneVariables", order = 0)]
    public class SceneVariables : ScriptableObject {

        public StringVariables strings;
        public FloatVariables floats;
        public BooleanVariables bools;
        public IntegerVariables integers;

    #region String
        public bool AddString(string key, string value){
            return strings.AddVariable(key, value);
        }
        public void RemoveString(string key){
            strings.RemoveVariable(key);
        }
        public string GetString(string key){
            return strings.GetValue(key);
        }

        public void SetString(string key, string value){
            strings.SetValue(key, value);
        }

        public List<string> GetStringOptions(string key, out int index){
            return strings.GetOptions(key, out index);            
        }
    #endregion

    #region Float
        public bool AddFloat(string key, float value){
            return floats.AddVariable(key, value);
        }
        public void RemoveFloat(string key){
            floats.RemoveVariable(key);
        }
        public float GetFloat(string key){
            return floats.GetValue(key);
        }

        public void SetFloat(string key, float value){
            floats.SetValue(key, value);
        }

        public List<string> GetFloatOptions(string key, out int index){
            return floats.GetOptions(key, out index);            
        }

    #endregion

    #region Integer
        public bool AddInteger(string key, int value){
            return integers.AddVariable(key, value);
        }
        public void RemoveInteger(string key){
            integers.RemoveVariable(key);
        }
        public int GetInteger(string key){
            return integers.GetValue(key);
        }

        public void SetInteger(string key, int value){
            integers.SetValue(key, value);
        }

        public List<string> GetIntegerOptions(string key, out int index){
            return integers.GetOptions(key, out index);            
        }

    #endregion

    #region Boolean
        public bool AddBool(string key, bool value){
            return bools.AddVariable(key, value);
        }
        public void RemoveBool(string key){
            bools.RemoveVariable(key);
        }
        public bool GetBool(string key){
            return bools.GetValue(key);
        }

        public void SetBool(string key, bool value){
            bools.SetValue(key, value);
        }

        public List<string> GetBoolOptions(string key, out int index){
            return bools.GetOptions(key, out index);            
        }

    #endregion


        public void ResetToDefault(){
            strings.ResetToDefault();
            floats.ResetToDefault();
            bools.ResetToDefault();
            integers.ResetToDefault();
        }
    }
}
