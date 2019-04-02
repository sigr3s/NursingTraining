using System;
using NT;
using NT.SceneObjects;
using NT.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeInspector : GUIInspector {


    [Header("References")]    
    public TextMeshProUGUI title;
    public TextMeshProUGUI over;    

    [Header("Debug")]
    public ISceneObject current;

    private void Start() {
        SceneManager.Instance.OnCurrentChanged.AddListener(OnCurrentChanged);
    }

    private void OnCurrentChanged()
    {
        current = SceneManager.Instance.currentObject;

        object value =  SceneManager.Instance.sceneVariables.variableRepository.GetValue(current.GetName(), current.GetDataType());

        SetCurrent(current, value);
    }

    public void SetCurrent(ISceneObject dso, object Value, GameObject currentGo = null){
        current = dso;

        title.text = dso.GetName();

        Inspect(Value);
        
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }

    public void SetMouseOver(ISceneObject dso){
        over.text = dso.GetName();
    }
}