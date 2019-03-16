using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class RuntimeGraph : MonoBehaviour {
    [Header("Graph")]
    public NodeGraph graph;


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


    public ScrollRect scrollRect { get; private set; }

    private void Awake() {
        // Create a clone so we don't modify the original asset
        graph = graph.Copy();
        scrollRect = GetComponentInChildren<ScrollRect>();

        SpawnGraph();

        graphContextMenu.onClickSpawn -= SpawnNode;
        graphContextMenu.onClickSpawn += SpawnNode;
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

        for (int i = 0; i < graph.nodes.Count; i++) {
            Node node = graph.nodes[i];

            if(node == null) continue;

            UGUIBaseNode runtimeNode = null;

            runtimeNode = Instantiate(nodePrefab);

            runtimeNode.transform.SetParent(scrollRect.content);
            runtimeNode.node = node;
            runtimeNode.graph = this;
            runtimeNode.transform.localPosition = node.position;
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

}