using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct SurgicalInstrumentData{

    }

    [CreateAssetMenu(fileName = "SurgicalInstrument", menuName = "NT/Scene/SurgicalInstrument")]
    public class SurgicalInstrument : SceneObject<SurgicalInstrumentData> {
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "Grabbed" };
        }
    }
}