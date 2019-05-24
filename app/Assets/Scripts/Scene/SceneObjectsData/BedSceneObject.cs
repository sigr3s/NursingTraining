using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace NT.SceneObjects{
    public struct BedData{

        [Description("Bed Height")]
        public float bedHeight;
        public float tilt;
        public float temperature;
        public bool wheelsBlocked;
    }

    [CreateAssetMenu(fileName = "BedSceneObject", menuName = "NT/Scene/Bed")]
    public class BedSceneObject : SceneObject<BedData> {

    }
}