using System;
using UnityEngine;

namespace NT.SceneObjects{
    public struct DefibrilatorData{
        public bool hasCharge;
        public float configuredVoltage;
        public bool isCharging;
    }

    [Serializable]
    public class NTDefibrilator : NTSceneObject<DefibrilatorData>{

    }
}