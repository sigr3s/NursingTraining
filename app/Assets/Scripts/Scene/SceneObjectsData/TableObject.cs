using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct TebleData{
        public string h1;
        public string h2;
        public string h3;
        public string h4;
        public string h5;
        public string h6;
    }

    [CreateAssetMenu(fileName = "TableObject", menuName = "NT/Scene/Table")]
    public class TableObject : SceneObject<TebleData> {
        
    }
}