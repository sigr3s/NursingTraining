using System;
using System.Collections.Generic;
using NT.Graph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;
using XNode.InportExport;

public class RuntimeGraph : MonoBehaviour, IPointerClickHandler {
    [Header("Graph")]
    public NodeGraph graph;
    public string graphPath = "Import.json";


    [Header("Node Parts")]
    public GameObject inputPort;
    public GameObject outputPort;
    
    [Header("Node Properties")]
    public GameObject Property;
    public GameObject PropertyObject;

    [Header("References")]
    public UGUIContextMenu graphContextMenu;
    public UGUIContextMenu nodeContextMenu;    
    public Connection runtimeConnectionPrefab;
    public UGUIBaseNode nodePrefab;
    public UGUITooltip tooltip;


    public List<ColorByType> colors;

    [System.Serializable]
    public struct ColorByType{
        public string type;
        public Color color;
    }

    
    [Header("Debug")]
    public List<UGUIBaseNode> nodes;
    public List<UGUIBaseNode> selectedNodes;


    public ScrollRect scrollRect { get; private set; }

    private void Awake() {        
        scrollRect = GetComponentInChildren<ScrollRect>();

        graphContextMenu.onClickSpawn -= SpawnNode;
        graphContextMenu.onClickSpawn += SpawnNode;
    }

    public void SetGraph(NTGraph graph){
        Clear();
        this.graph = graph;
        SpawnGraph();
    }

    public virtual void SpawnNode(Type type, Vector2 position)
    {
        Node node = graph.AddNode(type);
        node.name = type.Name;
        node.position = position;
        Refresh();
    }

    public virtual void Refresh(){
        Clear();
		SpawnGraph();
    }

    public virtual void Clear() {
        for (int i = nodes.Count - 1; i >= 0; i--) {

            Destroy(nodes[i].gameObject);
        }
        nodes.Clear();
    }

    public void SpawnGraph() {
        if (nodes != null) nodes.Clear();
        else nodes = new List<UGUIBaseNode>();

        if(graph == null) return;

        for (int i = 0; i < graph.nodes.Count; i++) {
            Node node = graph.nodes[i];

            if(node == null) continue;

            UGUIBaseNode runtimeNode = null;

            runtimeNode = Instantiate(nodePrefab);

            runtimeNode.transform.SetParent(scrollRect.content);
            runtimeNode.node = node;
            runtimeNode.graph = this;
            runtimeNode.transform.localPosition = new Vector2(node.position.x , -node.position.y);
            runtimeNode.transform.localScale = Vector3.one;
            runtimeNode.name = node.name;

            runtimeNode.GetComponent<Image>().color = GetColorFor(node.GetType());

            runtimeNode.gameObject.SetActive(true);

            nodes.Add(runtimeNode);
        }
    }

    public virtual Color GetColorFor(Type t){
        foreach(ColorByType cbt in colors){
            if(t.AssemblyQualifiedName.Contains(cbt.type)){
                return cbt.color;
            }
        }

        return Color.white;
    }

    public virtual UGUIBaseNode GetRuntimeNode(Node node)
    {
        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].node == node) {
                return nodes[i];
            } else { }
        }
        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
				return;

        graphContextMenu.OpenAt(eventData.position);
    }
}