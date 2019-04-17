using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    [Serializable]
    public struct DummyData{
            
    }

    [CreateAssetMenu(fileName = "SceneObject", menuName = "NT/Scene/Dummy")]
    public class DummySceneObject : SceneObject<DummyData>
    {   

    }
}