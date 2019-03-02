using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTFloat : NTVariable<float>{
        public override void DeserializeDefaultValue(string data){ this.value = float.Parse(data); }

        public override void DeserializeValue(string data){ this.defaultValue = float.Parse(data); }

        public override string SerializeDefaultValue(){ return defaultValue.ToString(); }

        public override string SerializeValue(){ return value.ToString(); }
    }
}