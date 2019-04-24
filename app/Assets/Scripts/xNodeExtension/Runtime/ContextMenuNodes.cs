using System;
using NT;
using NT.Nodes.Variables;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuNodes : UGUIContextMenu {
    public GameObject prefabButton;


    public override void Initialize(){
        
        foreach(Type t in ReflectionUtilities.nodeTypes){
            
            if(t.IsAbstract || t.IsGenericType) continue;
            if( typeof(IVariableNode).IsAssignableFrom(t) ) continue;

            if(t == typeof(FlowNode) ||  t == typeof(NTNode)) continue;

            GameObject go = Instantiate(prefabButton, this.transform);
            go.GetComponentInChildren<Text>().text = t.ToString().Replace("NT.Nodes.", "");
            go.GetComponentInChildren<Button>().onClick.AddListener( () => {
                SpawnNode(t);
            });

        }
    }
}