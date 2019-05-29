using System;
using System.Collections.Generic;
using NT.SceneObjects;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSceneObjectsFilter : MonoBehaviour {
    public GameObject prototype;
    public Transform container;

    private Dictionary<string, GameObject> tabs = new Dictionary<string, GameObject>();
    private string[] filters;

    private MapEditor mapEditor;

    private void Start() {
        mapEditor = GetComponentInParent<MapEditor>();

        filters = Enum.GetNames( typeof(ObjectCategory) );

        for(int i = 0; i < filters.Length; i++){

            string filter = filters[i];
            GameObject tab = Instantiate(prototype, container);
            tab.name = filter;

            tab.GetComponent<Button>().onClick.AddListener( () => {
                TabClick(filter);
            });

            tab.GetComponentInChildren<TextMeshProUGUI>().text = filter.NicifyString();
            tab.gameObject.SetActive(true);

            tabs.Add(filter, tab);
        }

        TabClick(filters[0]);        
    }

    public void TabClick(string filter){
        foreach(string f in filters){
            if(f == filter){
                tabs[f].gameObject.GetComponent<Button>().interactable = false;
            }
            else
            {
                tabs[f].gameObject.GetComponent<Button>().interactable = true;                               
            }
        }

        ObjectCategory option = ObjectCategory.All;

        Enum.TryParse(filter, out option);

        mapEditor?.LoadObjectsButtons( option );
    }

   
}