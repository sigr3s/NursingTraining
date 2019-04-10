using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{

    [CreateAssetMenu(fileName = "Defibrilator", menuName = "NT/Scene/Defibrilator")]
    public class DefibrilatorSceneObject : SceneObject<NTDefibrilator> {
        public override List<string> GetCallbacks(){
            return new List<string>(){"OnCahrgeCompleted","OnDefibrilatorUsed"};
        }
    }
}