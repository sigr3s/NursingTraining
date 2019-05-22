using System;
using System.Collections.Generic;
using System.Linq;
using NT;
using NT.SceneObjects;
using UnityEngine;
using UnityEngine.UI;

public class GUIInspector : MonoBehaviour {
    [Header(" Prefabs ")]
    public GUIProperty property;
    public GUIPropertyObject propertyObject;


    [Header(" Scene references ")]
    public Transform content;


    [Header("Debug")]
    public List<GUIProperty> properties;
    public List<GUIPropertyObject> propertiesobjects;
    public object inspectingObject;
    Action<object> OnValueChanged;


    public static NTPool propertyPool;

    private void Awake() {
        if(propertyPool == null){
            GameObject p = new GameObject("PoolPropertys");
            propertyPool = p.AddComponent<NTPool>();
            propertyPool.Initialize(property.gameObject);
            
            DontDestroyOnLoad(propertyPool);
        }
    }

    public void CleanContent(){
        foreach (var property in properties)
        {
            propertyPool.PoolItem(property.gameObject);
        }

        foreach (var property in propertiesobjects)
        {
            Destroy(property.gameObject);
        }


        properties.Clear();
        propertiesobjects.Clear();
    }


    public void Inspect(object inspectObject, Action<object> OnValueChanged){
        CleanContent();

        if(inspectObject == null) return;

        inspectingObject = inspectObject;
        this.OnValueChanged = OnValueChanged;

        var degt = ReflectionUtilities.DesgloseInBasicTypes(inspectObject.GetType());

        Dictionary<string, GameObject> sublevels = new Dictionary<string,GameObject>();

        foreach (var deglossedType in degt)
        {
            foreach (var propertyPath in deglossedType.Value)
            {
                GameObject instancedProperty =  propertyPool.GetItem();   
                GUIProperty gp = instancedProperty.GetComponent<GUIProperty>();

                string[] path = propertyPath.Split('/');

                GameObject pathParent = content.gameObject;

                if(path.Length > 1){

                    string composedPath = "";

                    for(int i = 0; i < path.Length - 1; i++){
                        if(i == 0){
                            composedPath += path[i];

                            if(sublevels.ContainsKey(composedPath)){
                                pathParent = sublevels[composedPath];
                            }
                            else
                            {
                                pathParent = GameObject.Instantiate(propertyObject.gameObject);
                                pathParent.transform.SetParent(content);

                                GUIPropertyObject gpo = pathParent.GetComponent<GUIPropertyObject>();
                                gpo.propertyName.text = path[i];
                                pathParent = gpo.children.gameObject;

                                propertiesobjects.Add(gpo);
                                sublevels.Add(composedPath, pathParent);
                            }
                        }
                        else
                        {
                            composedPath += "/" + path[i];

                            if(sublevels.ContainsKey(composedPath)){
                                pathParent = sublevels[composedPath];
                            }
                            else
                            {
                                pathParent = GameObject.Instantiate(propertyObject.gameObject, pathParent.transform);
                            
                                GUIPropertyObject gpo = pathParent.GetComponent<GUIPropertyObject>();
                                gpo.propertyName.text = path[i];
                                pathParent = gpo.children.gameObject;
                                
                                propertiesobjects.Add(gpo);
                                sublevels.Add(composedPath, pathParent);
                            }
                        }
                    }
                }

                
                instancedProperty.transform.SetParent(pathParent.transform);

                properties.Add(gp);

                gp.OnValueChanged.RemoveAllListeners();                

                if(deglossedType.Key.IsString()){
                    gp.SetData( ReflectionUtilities.GetValueOf(path.ToList(), inspectObject), propertyPath, GUIProperty.PropertyType.String);
                }
                else if(deglossedType.Key.IsNumber())
                {
                    gp.SetData(ReflectionUtilities.GetValueOf(path.ToList(), inspectObject), propertyPath, GUIProperty.PropertyType.Number);
                    
                } 
                else if(deglossedType.Key.IsBool())
                {
                    gp.SetData(ReflectionUtilities.GetValueOf(path.ToList(), inspectObject), propertyPath, GUIProperty.PropertyType.Boolean);
                }
                else if(deglossedType.Key.IsEnum)
                {
                    gp.SetData(ReflectionUtilities.GetValueOf(path.ToList(), inspectObject), propertyPath, GUIProperty.PropertyType.Enumeration);
                }
                else
                {
                    gp.SetData(ReflectionUtilities.GetValueOf(path.ToList(), inspectObject), propertyPath, GUIProperty.PropertyType.SceneReference);
                }

                gp.OnValueChanged.RemoveAllListeners();
                gp.OnValueChanged.AddListener(OnPropertyChanged);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate( (RectTransform) content);
    }

    private void OnPropertyChanged(object value, string path)
    {
        List<string> variablePath = new List<string>(path.Split('/'));

        ReflectionUtilities.SetValueOf(ref inspectingObject, value, variablePath);

        OnValueChanged(inspectingObject);
    }
}