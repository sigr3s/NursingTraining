using System;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuGrouped : UGUIContextMenu {
    public GameObject prefabButton;

    public override void Initialize(){        
        GameObject remove = Instantiate(prefabButton, this.transform);
        
        remove.GetComponentInChildren<Text>().text = "Remove";
        remove.GetComponentInChildren<Button>().onClick.AddListener( () => {
            selected.Remove();
        });
    }

}