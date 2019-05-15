using System;
using System.Collections.Generic;
using System.Linq;
using NT;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class UGUIBaseNode :  MonoBehaviour, IDragHandler, IUGUINode, IContextItem {
    public Node node;
    public RuntimeGraph graph;

    public GameObject body;
    public List<UGUIPort> ports = new List<UGUIPort>();

    
    private Image baseImage;

    private void Awake() {
        if(node != null){
            List<string> ignored = new List<string>();

            foreach (NodePort port in node.Ports)
            {
                
                GameObject portGO =  Instantiate(port.direction == NodePort.IO.Input ? graph.inputPort : graph.outputPort, body.transform);

                portGO.transform.Find("Label").GetComponent<Text>().text = port.fieldName;
                UGUIPort guiport = portGO.GetComponentInChildren<UGUIPort>();
                guiport.fieldName = port.fieldName;
                guiport.node = node;
                guiport.name = port.fieldName;

                ports.Add(guiport);

                ignored.Add(port.fieldName);
            }

            transform.Find("Header/Title").GetComponent<Text>().text = node.name;


            var d = ReflectionUtilities.DesgloseInBasicTypes(node.GetType(), ignored);

            foreach(KeyValuePair<Type, List<string>> kvp in d){
                foreach(string variable in kvp.Value){
                    GameObject variableGo =  Instantiate(graph.Property, body.transform);


                    GUIProperty  gp = variableGo.GetComponent<GUIProperty>();

                    
                    object value = ReflectionUtilities.GetValueOf(variable.Split('/').ToList(), node);

                    if(kvp.Key.IsString()){
                        gp.SetData(value, variable, GUIProperty.PropertyType.String);
                    }
                    else if(kvp.Key.IsNumber())
                    {
                        gp.SetData(value, variable, GUIProperty.PropertyType.Number);
                        
                    } 
                    else if(kvp.Key.IsBool())
                    {
                        gp.SetData(value, variable, GUIProperty.PropertyType.Boolean);
                    }
                    else if(kvp.Key.IsEnum)
                    {
                        gp.SetData(value, variable, GUIProperty.PropertyType.Enumeration);
                    }

                    gp.OnValueChanged.RemoveAllListeners();
                    gp.OnValueChanged.AddListener(PropertyChanged);
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void PropertyChanged(object value, string path)
    {
        object n = node;
        ReflectionUtilities.SetValueOf(ref n, value, new List<string>(path.Split('/')) );
    }

    public void OnDrag(PointerEventData eventData) {

    }

    public virtual UGUIPort GetPort(string fieldName)
    {
        for (int i = 0; i < ports.Count; i++) {
            if (ports[i].name == fieldName) return ports[i];
        }
        return null;
    }

    public void SetColor()
    {
        GetComponent<Image>().color = graph.GetColorFor(node.GetType());
    }

    public virtual void UpdateGUI()
    {
        //throw new NotImplementedException();
    }

    private void LateUpdate() {
        foreach (UGUIPort port in ports) port.UpdateConnectionTransforms();
    }

    public bool HasNode(Node node)
    {
        return this.node == node;
    }

    public Node GetNode()
    {
        return this.node;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public RuntimeGraph GetRuntimeGraph()
    {
        return graph;
    }

    public void SetPosition(Vector2 position)
    {
        node.position = position;
    }

    public void RemoveNode()
    {
	    graph.graph.RemoveNode(node);
    }

    public void Remove()
    {
       RemoveNode();
    }

    public string GetKey()
    {
        return node.name;
    }
}