using NT;
using NT.SceneObjects;
using NT.Variables;
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
    public ISceneObject current;


    public void SetCurrent(ISceneObject dso, object Value, GameObject currentGo = null){
        current = dso;

        title.text = dso.GetName();

        var degt = ReflectionUtilities.DesgloseInBasicTypes(Value.GetType());

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

    public void SetMouseOver(ISceneObject dso){
        over.text = dso.GetName();
    }
}