using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{

    public struct ClosetData{
        public float MaxItems;
        public int numberOfItems;

        public Drawer d0;
        public Drawer d1;
        public Drawer d2;
            
    }

    [System.Serializable]
    public struct Drawer{
        public bool canbeOpened;
        public string slot00;
        public string slot01;
        public string slot02;
        public string slot03;
        public string slot04;
        public string slot05;
    }

    [CreateAssetMenu(fileName = "BedSceneObject", menuName = "NT/Scene/Closet")]
    public class ClosetSceneObject : SceneObject<ClosetData> {
        public override List<string> GetCallbacks(){
            return new List<string>(){"OnItemDropped","OnItemGetted"};
        }

        public override bool CanHoldItem(SceneGameObject obj){
            return true;
        }
    }
}