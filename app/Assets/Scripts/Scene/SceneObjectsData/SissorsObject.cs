using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct SissorsData{
        public float sharpness;
    }

    [CreateAssetMenu(fileName = "SissorsObject", menuName = "NT/Scene/Sissors")]
    public class SissorsObject : SceneObject<SissorsData> {
        
    }
}