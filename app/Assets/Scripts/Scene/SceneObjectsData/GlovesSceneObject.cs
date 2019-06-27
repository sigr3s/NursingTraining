using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NT.SceneObjects
{
    public struct GlovesData
    {
        
    }

    [CreateAssetMenu(fileName = "Gloves", menuName = "NT/Scene/Gloves")]
    public class GlovesSceneObject : SceneObject<GlovesData>
    {

        public override List<string> GetCallbacks()
        {
            return new List<string>() { "Gloves On" };
        }
    }
}
