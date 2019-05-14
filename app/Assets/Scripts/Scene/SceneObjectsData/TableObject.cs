using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct TebleData{
        public string slot1;
        public string slot2;
        public string slot3;
        public string slot4;
        public string slot5;
        public string slot6;
    }

    [CreateAssetMenu(fileName = "TableObject", menuName = "NT/Scene/Table")]
    public class TableObject : SceneObject<TebleData> {

        
        
    }
}