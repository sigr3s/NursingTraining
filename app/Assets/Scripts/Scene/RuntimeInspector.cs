using NT;
using NT.SceneObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeInspector : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject Property;

    [Header("References")]    
    public TextMeshProUGUI title;
    public TextMeshProUGUI over;
    public Transform properties;

    [Header("Debug")]
    public DummySceneObject current;


    public void SetCurrent(DummySceneObject dso, GameObject currentGo = null){
        current = dso;

        title.text = dso.name;

        var degt = ReflectionUtilities.DesgloseInBasicTypes(dso.GetType());

        if(properties.childCount > 0){
            for(int c = properties.childCount-1; c >= 0; c--){
                Transform child = properties.GetChild(c);
                Destroy(child.gameObject);
            }
        }


        foreach(var dkv in degt){
            foreach(var vs in dkv.Value){
                var p = Instantiate(Property, properties);
                p.GetComponentInChildren<Text>().text = vs;
            }
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }

    public void SetMouseOver(DummySceneObject dso){
        over.text = dso.name;
    }
}