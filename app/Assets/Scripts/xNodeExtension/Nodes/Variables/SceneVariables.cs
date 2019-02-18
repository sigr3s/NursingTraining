using System;
using System.Collections.Generic;
using System.Reflection;
using NT.Graph;
using UnityEngine;


namespace NT.Variables
{
    [CreateAssetMenu(fileName = "SceneVariables", menuName = "NT/SceneVariables", order = 0)]
    public class SceneVariables : ScriptableObject {
        [SerializeField] public NTVariableRepository variableRepository;

        [Header("Colors")]
        [SerializeField] public TypeColorDictionary typeColorDict;
        [SerializeField] public Color DefaultColor;

        public SceneVariables(){
            typeColorDict = new TypeColorDictionary();
            
            typeColorDict.Add(typeof(NTString), VariablesColors.StringColor );
            typeColorDict.Add(typeof(NTFloat),  VariablesColors.FloatColor );
            typeColorDict.Add(typeof(NTBool),   VariablesColors.BoolColor );
            typeColorDict.Add(typeof(NTInt),    VariablesColors.IntColor );
        }

        public Color GetColorFor(Type type)
        {
            Color val;
            if(typeColorDict != null && typeColorDict.TryGetValue(type, out val))
                return val;

            return DefaultColor;
        }
    }


    [Serializable]
    public class TypeColorDictionary : Dictionary<Type, Color>, ISerializationCallbackReceiver{
        [SerializeField] public List<string> keys = new List<string>();
        [SerializeField] public List<Color> values = new List<Color>();

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

            if (keys.Count != values.Count){
                return;
            }
            Assembly asm = typeof(NTNode).Assembly;

            for (int i = 0; i < keys.Count; i++)
                this.Add(asm.GetType(keys[i]), values[i]);
        }
    }
}
