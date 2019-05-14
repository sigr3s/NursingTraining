using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetSetContextMenu : UGUIContextMenu {
    public GameObject prefabButton;

    public Action GetAction;
    public Action SetAction;
    
    public override void Initialize(){
        GameObject getNode = Instantiate(prefabButton, this.transform);
        
        getNode.GetComponentInChildren<Text>().text = "Get";
        getNode.GetComponentInChildren<Button>().onClick.AddListener( () => {
            GetAction?.Invoke();
            Close();
        });

        GameObject setNode = Instantiate(prefabButton, this.transform);
        
        setNode.GetComponentInChildren<Text>().text = "Set";
        setNode.GetComponentInChildren<Button>().onClick.AddListener( () => {
            SetAction?.Invoke();
            Close();
        });
    }

}
