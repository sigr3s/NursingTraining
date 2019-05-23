using System;
using System.Collections;
using System.Collections.Generic;
using NT;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class UGUIPort : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {

	public string fieldName;
	public GUIProperty backingValue;
	[HideInInspector] public XNode.Node node;

	public GUIProperty.PropertyType propertyType;
	private bool isBasicTypePort = false;
	private NodePort port;
	private Connection tempConnection;
	private NodePort startPort;
	private UGUIPort tempHovered;
	private RuntimeGraph graph;
	private Vector2 startPos;
	private List<Connection> connections = new List<Connection>();

	void Start() {
		if(node == null){
			Destroy(this);
			return;
		}

		port = node.GetPort(fieldName);
		graph = GetComponentInParent<RuntimeGraph>();
		if (port.IsOutput && port.IsConnected) {
			for (int i = 0; i < port.ConnectionCount; i++) {
				AddConnection();
			}
		}

		if(port.ValueType == typeof(bool) ){
			transform.parent.GetComponent<Image>().color = Color.green;
			isBasicTypePort = true;
			propertyType = GUIProperty.PropertyType.Boolean;
		}
		else if(port.ValueType == typeof(string) ){
			transform.parent.GetComponent<Image>().color = Color.magenta;
			isBasicTypePort = true;
			propertyType = GUIProperty.PropertyType.String;
		}
		else if(port.ValueType.IsNumber()){
			transform.parent.GetComponent<Image>().color = Color.yellow;
			isBasicTypePort = true;
			propertyType = GUIProperty.PropertyType.Number;
		}
		else if(port.ValueType == typeof(DummyConnection))
		{
			transform.parent.GetComponent<Image>().color = Color.white;
		}
		else if(port.ValueType == typeof(SceneGameObject))
		{
			transform.parent.GetComponent<Image>().color = Color.red;
		}
		else
		{
			transform.parent.GetComponent<Image>().color = Color.blue;
		}

		if(backingValue != null) backingValue.gameObject.SetActive(false);
	}

	void Reset() {
		fieldName = name;
	}

	private void OnDestroy() {
		// Also destroy connections
		for (int i = connections.Count - 1; i >= 0; i--) {
			Destroy(connections[i].gameObject);
		}
		connections.Clear();
	}

	private void Update() {
		if(backingValue != null && isBasicTypePort && port.IsInput){

			if(!port.IsConnected && !backingValue.gameObject.activeInHierarchy){
				backingValue.gameObject.SetActive(true);

				if(port.IsStatic){
					backingValue.SetData(ReflectionUtilities.GetValueOf(new List<string>(){fieldName}, node) , fieldName, propertyType);
					backingValue.OnValueChanged.AddListener(ValueChanged);
				}
				else if(node is NTNode)
				{
					NTNode n = (NTNode) node;
					object val = n.GetInstancePortValue(fieldName);
					val = val != null ? val : Activator.CreateInstance(port.ValueType);

					backingValue.SetData(val , fieldName, propertyType);
					backingValue.OnValueChanged.AddListener(InstanceValueChanged);
				}
			}
			else if(port.IsConnected && backingValue.gameObject.activeInHierarchy)
			{
				backingValue.gameObject.SetActive(false);
				backingValue.OnValueChanged.RemoveListener(ValueChanged);
			}

		}
	}
		private void InstanceValueChanged(object value, string path)
    {
			 NTNode n = (NTNode) node;
			 n.SetInstancePortValue(path, value);
    }

    private void ValueChanged(object value, string path)
    {
			object n = node;
			ReflectionUtilities.SetValueOf(ref n, value, new List<string>(path.Split('/')) );
    }

    public void UpdateConnectionTransforms() {
		if (port.IsInput) return;

		while (connections.Count < port.ConnectionCount) AddConnection();
		while (connections.Count > port.ConnectionCount) {
			Destroy(connections[0].gameObject);
			connections.RemoveAt(0);
		}

		// Loop through connections
		for (int i = port.ConnectionCount - 1 ; i >= 0; i--) {
			NodePort other = port.GetConnection(i);
			IUGUINode otherNode = graph.GetRuntimeNode(other.node);
			if (otherNode == null){
				Debug.LogWarning(other.node.name + " node not found ||| " + other.node, this);
				port.Disconnect(i);
				continue;
			}
			
			Transform port2 = otherNode.GetPort(other.fieldName, other.node).transform;
			if (!port2) Debug.LogWarning(other.fieldName + " not found", this);
			connections[i].SetPosition(transform.position, port2.position);
		}
	}

	private void AddConnection() {
		Connection connection = Instantiate(graph.runtimeConnectionPrefab);
		connection.transform.SetParent(graph.scrollRect.content);
		connections.Add(connection);
		connection.name = fieldName + "_ connect";
	}

	public void OnBeginDrag(PointerEventData eventData) {
		if (port.IsOutput) {
			tempConnection = Instantiate(graph.runtimeConnectionPrefab);
			tempConnection.transform.SetParent(graph.scrollRect.content);
			tempConnection.SetPosition(transform.position, eventData.position);
			startPos = transform.position;
			startPort = port;
		} else {
			if (port.IsConnected) {
				NodePort output = port.Connection;
				
				IUGUINode otherNode = graph.GetRuntimeNode(output.node);
				UGUIPort otherUGUIPort = otherNode.GetPort(output.fieldName, output.node);
				
				output.Disconnect(port);
				tempConnection = Instantiate(graph.runtimeConnectionPrefab);
				tempConnection.transform.SetParent(graph.scrollRect.content);
				tempConnection.SetPosition(otherUGUIPort.transform.position, eventData.position);
				startPos = otherUGUIPort.transform.position;
				startPort = otherUGUIPort.port;
				graph.GetRuntimeNode(node).UpdateGUI();
			}
		}
	}

	public void OnDrag(PointerEventData eventData) {
		if (tempConnection == null) return;
		UGUIPort otherPort = FindPortInStack(eventData.hovered);
		tempHovered = otherPort;
		tempConnection.SetPosition(startPos, eventData.position);
	}

	public void OnEndDrag(PointerEventData eventData) {
		if (tempConnection == null) return;
		if (tempHovered) {
			startPort.Connect(tempHovered.port);
			graph.GetRuntimeNode(tempHovered.node).UpdateGUI();
		}
		Destroy(tempConnection.gameObject);
	}

	public UGUIPort FindPortInStack(List<GameObject> stack) {
		for (int i = 0; i < stack.Count; i++) {
			UGUIPort port = stack[i].GetComponent<UGUIPort>();
			if (port) return port;
		}
		return null;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		graph.tooltip.Show();
		object obj = node.GetInputValue<object>(port.fieldName, null);
		if (obj == null){
		  obj =	node.GetValue(port);
		}
		
		
		if(obj != null) graph.tooltip.label.text = obj.ToString();
		else graph.tooltip.label.text = "n/a";
	}

	public void OnPointerExit(PointerEventData eventData) {
		graph.tooltip.Hide();
	}
}