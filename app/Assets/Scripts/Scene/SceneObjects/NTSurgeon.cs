using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{
    [Serializable]
    public struct SurgeonData{
        public string currentObject;
        public string lastSaid;
        public float elements;
        public SurgeonDAT dat;
    }


    [Serializable]
    public struct SurgeonDAT{
        public string lastSaid2;
    }

    [Serializable]
    public class NTSurgeon : NTSceneObject<SurgeonData>{

    }
}