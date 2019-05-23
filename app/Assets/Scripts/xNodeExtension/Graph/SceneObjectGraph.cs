using System.Collections.Generic;
using UnityEngine;

namespace NT.Graph{

    [System.Serializable]
    public class SceneObjectGraph : NTGraph
    {
        public string linkedNTVariable;
        public SceneGameObject assignedSCGO = null;
        public string displayName = "";

        public override List<string> GetCallbacks(){

            assignedSCGO = SessionManager.Instance.GetSceneGameObject(linkedNTVariable);

            if(assignedSCGO != null){
                return assignedSCGO.sceneObject.GetCallbacks();
            }
            else{
                return new List<string>();
            }
        }
    }
}