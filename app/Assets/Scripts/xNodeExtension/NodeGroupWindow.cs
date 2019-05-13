using System.Collections;
using System.Collections.Generic;
using NT.Graph;
using TMPro;
using UnityEngine;
using XNode;

public class NodeGroupWindow : Singleton<NodeGroupWindow>{
    public TMP_InputField nameInputField;

    List<Node> nodesToGroup;
    NodeGraph graph;

    private void Start() {
        var a = Instance;
        gameObject.SetActive(false);
    }

    public void Open(List<Node> nodesToGroup, NodeGraph graph)
    {
        this.nodesToGroup = nodesToGroup;
        this.graph = graph;

        gameObject.SetActive(true);
    }

    public void Close(){
        gameObject.SetActive(false);
    }

    public void CreateGroup(){
        NodeGroupGraph.GroupNodes(nodesToGroup, graph, nameInputField.text);
        SessionManager.Instance.OnShowingGraphChanged.Invoke();
        Close();
    }

}
