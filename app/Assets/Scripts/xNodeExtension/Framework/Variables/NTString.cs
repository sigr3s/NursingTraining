using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTString : NTVariable<string>{

        public override void DeserializeDefaultValue(string data){ this.value = data; }

        public override void DeserializeValue(string data){ this.defaultValue = data; }

        public override string SerializeDefaultValue(){ return defaultValue; }

        public override string SerializeValue(){ return value; }

        public override object GetValue(){
            if(this.value == null) return "";
            return base.GetValue();
        }
    }
}