using System;
using System.Collections;
using System.Collections.Generic;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;
using UnityEngine.UI;

public class MapUIController : MonoBehaviour
{
    public SceneObjects objects;
    public GameObject objectUi;


    [Header("Scene References")]
    public Transform objectList;
    public MapEditor editor;


    private void Awake() {
        foreach (var so in objects.objectSet)
        {
            ISceneObject isc = (ISceneObject) so;

            if(isc == null) continue;

            UISceneObject uisc = isc.GetUISceneObject();
            
            GameObject soButton = Instantiate(objectUi, objectList);            
            Button button =  soButton.GetComponent<Button>();

            button.onClick.AddListener(() =>{ editor.SetPlacementObject(isc); });

            button.targetGraphic.color = uisc.color;
            soButton.transform.Find("Icon").GetComponent<Image>().sprite = uisc.icon;

        }
    }
}
