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
    public SceneGameObject current;

    private void Start() {
        SessionManager.Instance.OnCurrentChanged.AddListener(OnCurrentChanged);
    }

    private void OnCurrentChanged()
    {
        current = SessionManager.Instance.selectedSceneObject;
        object value = null; 

        if(current != null){
            value = SessionManager.Instance.sceneVariables.variableRepository.GetDefaultValue(current.NTKey, current.NTDataType);
            SetCurrent(current.NTKey, value);
        }
        else
        {
            Inspect(null, null);
        }

    }

    public void SetCurrent(string name, object Value, GameObject currentGo = null){
        title.text = name;

        Inspect(Value, OnChanged);
        
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    private void OnChanged(object value)
    {
        SessionManager.Instance.sceneVariables.variableRepository.SetDefaultValue(current.NTDataType, current.NTKey, value);
    }
}