using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTInt : NTVariable<int>{
        public override void DeserializeDefaultValue(string data){ this.value = int.Parse(data); }

        public override void DeserializeValue(string data){ this.defaultValue = int.Parse(data); }

        public override string SerializeDefaultValue(){ return defaultValue.ToString(); }

        public override string SerializeValue(){ return value.ToString(); }
    }
}