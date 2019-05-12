
using System.Collections.Generic;
using NT.Graph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class UGUIGroupedNode :  MonoBehaviour, IDragHandler, IUGUINode {
    public NodeGroupGraph group;
    public RuntimeGraph graph;

    public GameObject body;
    public List<UGUIPort> ports = new List<UGUIPort>();
    private Image baseImage;

    private void Awake() {
        if(group != null){
            foreach (NodePort port in group.ports)
            {
                GameObject portGO =  Instantiate(port.direction == NodePort.IO.Input ? graph.inputPort : graph.outputPort, body.transform);

                portGO.transform.Find("Label").GetComponent<Text>().text = port.fieldName;
                UGUIPort guiport = portGO.GetComponentInChildren<UGUIPort>();
                guiport.fieldName = port.fieldName;
                guiport.node = port.node;
                guiport.name = port.fieldName;

                ports.Add(guiport);

                if(port.IsDynamic){
                    Debug.LogWarning("Instantiate dynamic port!");
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
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
        //throw new NotImplementedException();
    }

    private void LateUpdate() {
        foreach (UGUIPort port in ports) port.UpdateConnectionTransforms();
    }

    public bool HasNode(Node node)
    {
        return group.nodes.Contains(node);
    }

    public Node GetNode()
    {
        return null;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}