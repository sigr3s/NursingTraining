using UnityEngine;
using XNode;

public interface IUGUINode {
    void UpdateGUI();
    UGUIPort GetPort(string fieldName);
    bool HasNode(Node node);
    Node GetNode(); 
    GameObject GetGameObject();
    RuntimeGraph GetRuntimeGraph();
    void SetPosition(Vector2 position);
    void RemoveNode();
}