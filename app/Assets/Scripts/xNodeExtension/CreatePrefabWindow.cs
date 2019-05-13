using System;
using System.Collections;
using System.Collections.Generic;
using NT.SceneObjects;
using UnityEngine;
using TMPro;

public class CreatePrefabWindow : Singleton<CreatePrefabWindow>
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown iconSelector;
    IContextItem selected;

    private void Start() {
        var a = Instance;
        gameObject.SetActive(false);
    }
    public void Open(IContextItem selected)
    {
        this.selected = selected;
        gameObject.SetActive(true);
    }

    public void Close(){
        gameObject.SetActive(false);
    }

    public void CreatePrefab(){
        PrefabObject.CreatePrefab(  Guid.NewGuid().ToString() , 
                                    SessionManager.Instance.GetSceneGameObject(selected.GetKey()),
                                    nameInputField.text,
                                    iconSelector.value );           
        SessionManager.Instance.sceneObjects.LoadPrefabs();
        SessionManager.Instance.mapLoader.ReloadUI();

        Close();
    }
}
