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
    public SceneObjectCollider current;

    private void Start() {
        SessionManager.Instance.OnCurrentChanged.AddListener(OnCurrentChanged);
    }

    private void OnCurrentChanged()
    {
        current = SessionManager.Instance.selectedSceneObject;

        object value =  SessionManager.Instance.sceneVariables.variableRepository.GetValue(current.NTKey, current.NTDataType);

        SetCurrent(current.NTKey, value);
    }

    public void SetCurrent(string name, object Value, GameObject currentGo = null){
        title.text = name;

        Inspect(Value);
        
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }
}