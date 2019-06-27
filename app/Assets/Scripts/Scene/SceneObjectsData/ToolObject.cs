using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct ToolData{
        public float sharpness;
    }

    [CreateAssetMenu(fileName = "ToolObject", menuName = "NT/Scene/Tool")]
    public class ToolObject : SceneObject<ToolObject> {
        
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "Grabbed" };
        }
    }
}