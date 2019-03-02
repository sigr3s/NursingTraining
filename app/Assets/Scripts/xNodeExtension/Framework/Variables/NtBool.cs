using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTBool : NTVariable<bool>{
        public override void DeserializeDefaultValue(string data){ this.value = bool.Parse(data); }

        public override void DeserializeValue(string data){ this.defaultValue = bool.Parse(data); }

        public override string SerializeDefaultValue(){ return defaultValue.ToString(); }

        public override string SerializeValue(){ return value.ToString(); }
    }
}