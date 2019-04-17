using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct DefibrilatorData{
        public bool hasCharge;
        public float configuredVoltage;
        public bool isCharging;
    }

    [CreateAssetMenu(fileName = "Defibrilator", menuName = "NT/Scene/Defibrilator")]
    public class DefibrilatorSceneObject : SceneObject<DefibrilatorData> {
        public override List<string> GetCallbacks(){
            return new List<string>(){"OnCahrgeCompleted","OnDefibrilatorUsed"};
        }
    }
}