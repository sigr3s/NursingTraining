using System.Collections;
using System.Collections.Generic;
using NT.Graph;
using NT.Nodes.Messages;
using UnityEngine;
using XNode;

public class CallbackHierarchy : GUIHierarchy
{
    private void Start() {
        Rebuild();
        SessionManager.Instance.OnShowingGraphChanged.AddListener(Rebuild);
    }

    private void OnEnable() {
        Rebuild();
    }

    public override List<HierarchyModel> GetRoot(){        
        List<HierarchyModel> root = new List<HierarchyModel>();
        
        NodeGraph showing = SessionManager.Instance.showingGraph;
        
        if(showing is NTGraph){
            List<string> callbacks =  ((NTGraph) showing ).GetCallbacks();

            foreach (var callback in callbacks)
            {
                root.Add(new HierarchyModel(
                        new NodeHierarchyData{ 
                            name = callback, 
                            key = callback, 
                            onNodeCreated = (n) =>{
                                ( (CallbackNode) n).key = callback;
                                ( (CallbackNode) n).linkedToSceneObject = showing is SceneObjectGraph ? ((SceneObjectGraph) showing).linkedNTVariable : "";
                        },
                        nodeType = typeof(CallbackNode)}
                ));
                
            }   
        }
        return root;
    }
}
