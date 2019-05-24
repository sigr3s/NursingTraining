using System;
using NT;
using NT.Nodes.Variables;
using UnityEngine;
using UnityEngine.UI;
using XNode;
using TMPro;

public class ContextMenuNodes : UGUINodeContextMenu {
    public GameObject prefabButton;


    public override void Initialize(){

        foreach(Type t in ReflectionUtilities.nodeTypes){

            if(t.IsAbstract || t.IsGenericType) continue;
            if( typeof(IVariableNode).IsAssignableFrom(t) ) continue;

            if(t == typeof(FlowNode) ||  t == typeof(NTNode)) continue;

            Node n = (Node) Activator.CreateInstance(t);

            string name = t.Name;

            if(n is NTNode){
                name =  ( (NTNode) n).GetDisplayName();
            }

            GameObject go = Instantiate(prefabButton, this.transform);
            go.GetComponentInChildren<TextMeshProUGUI>().text = name;
            go.GetComponentInChildren<Button>().onClick.AddListener( () => {
                SpawnNode(t);
            });

        }
    }
}