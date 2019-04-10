using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{

    [CreateAssetMenu(fileName = "BedSceneObject", menuName = "NT/Scene/Closet")]
    public class ClosetSceneObject : SceneObject<NTCloset> {
        public override List<string> GetCallbacks(){
            return new List<string>(){"OnItemDropped","OnItemGetted"};
        }

        public override bool CanHoldItem(SceneGameObject obj){
            return true;
        }
    }
}