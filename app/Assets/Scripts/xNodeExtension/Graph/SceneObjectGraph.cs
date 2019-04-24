using System.Collections.Generic;
using UnityEngine;

namespace NT.Graph{
    [CreateAssetMenu(fileName = "Scene Object Graph", menuName = "NT/Scene Object Graph")]
    public class SceneObjectGraph : NTGraph
    {
        public string linkedNTVariable;
        public SceneGameObject assignedSCGO = null;

        public override List<string> GetCallbacks(){

            assignedSCGO = SessionManager.Instance.GetSceneGameObject(linkedNTVariable);
            
            if(assignedSCGO != null){
                return assignedSCGO.sceneObject.GetCallbacks();
            }
            else{
                return new List<string>();
            }
        }

        public override void LoadFromGraph(NTGraph g){
            base.LoadFromGraph(g);
            linkedNTVariable = ((SceneObjectGraph) g).linkedNTVariable;
        }
    }
}