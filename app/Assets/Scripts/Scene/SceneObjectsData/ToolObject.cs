using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct ToolData{
        public float sharpness;
    }

    [CreateAssetMenu(fileName = "ToolObject", menuName = "NT/Scene/Tool")]
    public class ToolObject : SceneObject<ToolObject> {
        
    }
}