using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContextMenuHierarchy : UGUIContextMenu {
    public GameObject prefabButton;

    public override void Initialize(){
        GameObject createPrefab = Instantiate(prefabButton, this.transform);
        
        createPrefab.GetComponentInChildren<TextMeshProUGUI>().text = "Create Prefab";
        createPrefab.GetComponentInChildren<Button>().onClick.AddListener( () => {
            PrefabDialog();
            Close();
        });

        GameObject remove = Instantiate(prefabButton, this.transform);
        
        remove.GetComponentInChildren<TextMeshProUGUI>().text = "Remove";
        remove.GetComponentInChildren<Button>().onClick.AddListener( () => {
            selected.Remove();
            Close();
        });
    }

    private void PrefabDialog()
    {
        CreatePrefabWindow.Instance.Open(selected);
    }
}