using System;
using System.Collections.Generic;
using System.Linq;
using NT;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class UGUIBaseNode :  MonoBehaviour, IDragHandler {
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
                    GameObject variableGo =  Instantiate(graph.stringPtoperty, body.transform);


                    GUIProperty  gp = variableGo.GetComponent<GUIProperty>();

                    gp.SetData(ReflectionUtilities.GetValueOf(variable.Split('/').ToList(), node).ToString(), variable, GUIProperty.PropertyType.String );
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }


        //baseImage =  GetComponent<Image>();

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

    public virtual void UpdateGUI()
    {
        throw new NotImplementedException();
    }

    private void LateUpdate() {
        foreach (UGUIPort port in ports) port.UpdateConnectionTransforms();
    }
}