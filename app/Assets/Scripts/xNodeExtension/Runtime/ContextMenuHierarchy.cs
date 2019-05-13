using System;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuHierarchy : UGUIContextMenu {
    public GameObject prefabButton;

    public override void Initialize(){
        GameObject createPrefab = Instantiate(prefabButton, this.transform);
        
        createPrefab.GetComponentInChildren<Text>().text = "Create Prefab";
        createPrefab.GetComponentInChildren<Button>().onClick.AddListener( () => {
            PrefabDialog();
        });

        GameObject remove = Instantiate(prefabButton, this.transform);
        
        remove.GetComponentInChildren<Text>().text = "Remove";
        remove.GetComponentInChildren<Button>().onClick.AddListener( () => {
            selected.Remove();
        });
    }

    private void PrefabDialog()
    {
        CreatePrefabWindow.Instance.Open(selected);
    }
}