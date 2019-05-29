
using System.Collections.Generic;
using NT.Graph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;
using TMPro;

public class UGUIGroupedNode :  MonoBehaviour, IPointerClickHandler, IDragHandler, IUGUINode, IContextItem {
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

                portGO.transform.Find("Label").GetComponent<Text>().text = port.fieldName.NicifyString();
                UGUIPort guiport = portGO.GetComponentInChildren<UGUIPort>();
                guiport.fieldName = port.fieldName;
                guiport.node = port.node;
                guiport.name = port.fieldName;

                if(port.IsConnected && port.Connection == null){
                    Debug.Log("Lost some connection" + " ___ " + port.fieldName);
                }

                ports.Add(guiport);

                if(port.IsDynamic){
                    Debug.LogWarning("Instantiate dynamic port!");
                }
            }

            transform.Find("Header/Title").GetComponent<TextMeshProUGUI>().text = group.name;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData) {

    }

    public void OnPointerClick(PointerEventData eventData){
        SessionManager.Instance.showingGraph = group;
    }


    public UGUIPort GetPort(string fieldName, Node n)
    {
        for (int i = 0; i < ports.Count; i++) {
            if (ports[i].name == fieldName && ports[i].node == n) return ports[i];
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

    public RuntimeGraph GetRuntimeGraph()
    {
        return graph;
    }

    public void SetPosition(Vector2 position)
    {
        group.position = position;
    }

    public void RemoveNode()
    {
        ((NTGraph) graph.graph).RemoveGroupedNodes(group);
    }

    public void Remove()
    {
        RemoveNode();
    }

    public string GetKey()
    {
        return group.name;
    }
}