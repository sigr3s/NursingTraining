using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class UGUIBaseNode :  MonoBehaviour, IDragHandler {
    public Node node;
    public RuntimeGraph graph;


    public GameObject inputPort;
    public GameObject outputPort;
    public GameObject body;

    public List<UGUIPort> ports = new List<UGUIPort>();

    
    private Image baseImage;

    private void Awake() {
        if(node != null){
            foreach (NodePort port in node.Ports)
            {
                
                GameObject portGO =  Instantiate(port.direction == NodePort.IO.Input ? inputPort : outputPort, body.transform);

                portGO.transform.Find("Label").GetComponent<Text>().text = port.fieldName;
                UGUIPort guiport = portGO.GetComponentInChildren<UGUIPort>();
                guiport.fieldName = port.fieldName;
                guiport.node = node;
                guiport.name = port.fieldName;

                ports.Add(guiport);
            }

            transform.Find("Header/Title").GetComponent<Text>().text = node.name;

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