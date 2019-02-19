using System;
using UnityEngine;

namespace NT.Variables
{
    [Serializable]
    public struct SurgeonData{
        public string currentObject;
        public string lastSaid;
    }

    [Serializable]
    public class NTSurgeon : NTVariable<SurgeonData>{    }
}