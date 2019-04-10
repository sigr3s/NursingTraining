using System;
using UnityEngine;

namespace NT.SceneObjects{
    public struct BedData{
        public float bedHeight;
        public float tilt;
        public float temperature;
        public bool wheelsBlocked;
    }

    [Serializable]
    public class NTBed : NTSceneObject<BedData>{

    }
}