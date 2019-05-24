using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetSetContextMenu : UGUIContextMenu {
    public GameObject prefabButton;

    public Action GetAction;
    public Action SetAction;
    
    public override void Initialize(){
        GameObject getNode = Instantiate(prefabButton, this.transform);
        
        getNode.GetComponentInChildren<TextMeshProUGUI>().text = "Get";
        getNode.GetComponentInChildren<Button>().onClick.AddListener( () => {
            GetAction?.Invoke();
            Close();
        });

        GameObject setNode = Instantiate(prefabButton, this.transform);
        
        setNode.GetComponentInChildren<TextMeshProUGUI>().text = "Set";
        setNode.GetComponentInChildren<Button>().onClick.AddListener( () => {
            SetAction?.Invoke();
            Close();
        });
    }

}
