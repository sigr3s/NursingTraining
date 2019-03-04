using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{
    [Serializable]
    public struct SurgeonData{
        public string currentObject;
        public string lastSaid;

        public List<string> myList; // = new List<string>();
        
    }

    [Serializable]
    public class NTSurgeon : NTSceneObject<SurgeonData>{

    }
}