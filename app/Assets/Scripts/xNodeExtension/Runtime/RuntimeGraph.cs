using System;
using System.Collections.Generic;
using System.IO;
using NT.Graph;
using OdinSerializer;
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
    public UGUINodeContextMenu graphContextMenu;
    public UGUINodeContextMenu nodeContextMenu;    
    public Connection runtimeConnectionPrefab;
    public UGUIBaseNode nodePrefab;    
    public UGUIGroupedNode groupedNodePrefab;

    public UGUITooltip tooltip;


    public List<ColorByType> colors;

    [System.Serializable]
    public struct ColorByType{
        public string type;
        public Color color;
    }

    
    [Header("Debug")]
    public List<IUGUINode> nodes = new List<IUGUINode>();
    public List<IUGUINode> selectedNodes  = new List<IUGUINode>();


    public ScrollRect scrollRect { get; private set; }

    private void Awake() {        
        scrollRect = GetComponentInChildren<ScrollRect>();

        graphContextMenu.onClickSpawn -= SpawnNode;
        graphContextMenu.onClickSpawn += SpawnNode;
    }

    public void SetGraph(NodeGraph graph){
        Clear();
        this.graph = graph;
        scrollRect.content.localPosition = Vector2.zero;
        SpawnGraph();
    }

    public virtual void SpawnNode(Type type, Vector2 position)
    {
        Node node = graph.AddNode(type);
        node.position = position;
        Refresh();
    }

    public void GroupSelected()
    {
        List<Node> nodesToGroup = new List<Node>();
        foreach(var selectedNode in selectedNodes){
            nodesToGroup.Add(selectedNode.GetNode());
        }
        
        NodeGroupWindow.Instance.Open(nodesToGroup, graph);
    }

    public virtual void Refresh(){
        Clear();
		SpawnGraph();
    }

    public virtual void Clear() {
        for (int i = nodes.Count - 1; i >= 0; i--) {
            Destroy(nodes[i].GetGameObject());
        }
        nodes.Clear();
    }

    public void SpawnGraph() {
        if (nodes != null) nodes.Clear();
        else nodes = new List<IUGUINode>();

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
            runtimeNode.name = node.GetType().Name;

            runtimeNode.SetColor();

            runtimeNode.gameObject.SetActive(true);

            nodes.Add(runtimeNode);
        }
    
        if(graph is NTGraph){
            NTGraph gnt = (NTGraph) graph;

            foreach(var extra in gnt.packedNodes){
                UGUIGroupedNode runtimeNode = null;

                runtimeNode = Instantiate(groupedNodePrefab);

                runtimeNode.transform.SetParent(scrollRect.content);
                runtimeNode.group = extra;
                runtimeNode.graph = this;
                runtimeNode.transform.localPosition = new Vector2(extra.position.x , -extra.position.y);
                runtimeNode.transform.localScale = Vector3.one;
                runtimeNode.name = "Extra thing????";

                runtimeNode.GetComponent<Image>().color = Color.green;
                runtimeNode.gameObject.SetActive(true);
                nodes.Add(runtimeNode);
            }
        }
    }

    public void AddNodeGroup(NodeGroupGraph nhd, Vector2 nodePosition)
    {
        if(graph is NTGraph){
            nhd.AddTo((NTGraph) graph, nodePosition);
            Refresh();
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

    public virtual IUGUINode GetRuntimeNode(Node node)
    {
        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].HasNode(node)) {
                return nodes[i];
            } else { }
        }
    
        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
				return;

        graphContextMenu.OpenAt(eventData.position, null);
    }
}