using System;
using UnityEngine;

namespace NT.Variables
{

    [Serializable]
    public class NTBool : NTVariable<bool>{
        public override void DeserializeDefaultValue(string data){bool.TryParse(data, out this.value); }

        public override void DeserializeValue(string data){ bool.TryParse(data, out this.defaultValue); }

        public override string SerializeDefaultValue(){ return defaultValue.ToString(); }

        public override string SerializeValue(){ return value.ToString(); }
    }
}