using System;
using UnityEngine;

namespace NT.SceneObjects
{
    [Serializable]
    public struct SurgeonData{
        public string currentObject;
        public string lastSaid;
    }

    [Serializable]
    public class NTSurgeon : NTSceneObject<SurgeonData>{

    }
}