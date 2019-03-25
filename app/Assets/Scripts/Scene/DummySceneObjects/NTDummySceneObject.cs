using System;

namespace NT.SceneObjects
{
    [Serializable]
    public struct DummyData{
        public string p0Content;
        
    }


    [Serializable]
    public class NTDummySceneObject : NTSceneObject<DummyData>{

    }
}