using System;
using System.Collections.Generic;
using System.Linq;
using NT;
using NT.SceneObjects;
using UnityEngine;

public class GUIInspector : MonoBehaviour {
    [Header(" Prefabs ")]
    public GUIProperty property;
    public GUIPropertyObject propertyObject;


    [Header(" Scene references ")]
    public Transform content;


    [Header("Debug")]
    public List<GUIProperty> properties;
    public List<GUIPropertyObject> propertiesobjects;

    public static NTPool propertyPool;

    public SurgeonData data;

    private void Awake() {
        if(propertyPool == null){
            GameObject p = new GameObject("PoolPropertys");
            propertyPool = p.AddComponent<NTPool>();
            propertyPool.Initialize(property.gameObject);
            
            DontDestroyOnLoad(propertyPool);
        }

        Inspect(data);
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


    public void Inspect(object o){
        CleanContent();

        if(o == null) return;

        var degt = ReflectionUtilities.DesgloseInBasicTypes(o.GetType());

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
                    gp.SetData( ReflectionUtilities.GetValueOf(path.ToList(), o), propertyPath, GUIProperty.PropertyType.String);
                }
                else if(deglossedType.Key.IsNumber())
                {
                    gp.SetData(ReflectionUtilities.GetValueOf(path.ToList(), o), propertyPath, GUIProperty.PropertyType.Number);
                    
                } 
                else
                {
                    gp.SetData(ReflectionUtilities.GetValueOf(path.ToList(), o), propertyPath, GUIProperty.PropertyType.Boolean);
                }

                gp.OnValueChanged.AddListener(OnPropertyChanged);
            }
        }
    }

    private void OnPropertyChanged(object arg0, string arg1)
    {
       Debug.Log("Property changed at  " + arg1);
    }
}