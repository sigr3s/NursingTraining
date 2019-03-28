using System;

namespace NT.SceneObjects
{
    [Serializable]
    public struct DummyData{
        public float MaxItems;
        public int numberOfItems;

        public Drawer d0;
        public Drawer d1;
        public Drawer d2;
            
    }

    public struct Drawer{
        public bool canbeOpened;
        public string slot00;
        public string slot01;
        public string slot02;
        public string slot03;
        public string slot04;
        public string slot05;
    }


    [Serializable]
    public class NTDummySceneObject : NTSceneObject<DummyData>{

    }
}