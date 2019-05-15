using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct TebleData{
        public SceneGameObject slot00;
        public SceneGameObject slot01;
        public SceneGameObject slot02;
        public SceneGameObject slot03;
        public SceneGameObject slot04;
        public SceneGameObject slot05;
    }

    [CreateAssetMenu(fileName = "TableObject", menuName = "NT/Scene/Table")]
    public class TableObject : SceneObject<TebleData> {

        
        
    }
}