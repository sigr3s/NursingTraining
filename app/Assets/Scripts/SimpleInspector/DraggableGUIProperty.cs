using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NT.Nodes.Variables;
using XNode;
using NT.SceneObjects;

public class DraggableGUIProperty : GUIProperty, IBeginDragHandler, IDragHandler, IEndDragHandler {
    
    private RuntimeGraph rg;

    
    public bool dragOnSurfaces = true;
    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;
    public GetSetContextMenu contextMenu;

    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = gameObject.FindInParents<Canvas>();
        if (canvas == null)
            return;
        
        m_DraggingIcon = new GameObject("icon");

        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();

        image.sprite = GetComponentInChildren<Image>(true).sprite;
        image.SetNativeSize();
        image.raycastTarget = false;

        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;

        m_DraggingIcon.SetDraggedPosition(eventData, m_DraggingPlane, dragOnSurfaces);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        if (m_DraggingIcon != null)
            m_DraggingIcon.SetDraggedPosition(eventData, m_DraggingPlane, dragOnSurfaces);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_DraggingIcon != null)
            Destroy(m_DraggingIcon);

        if(!eventData.pointerCurrentRaycast.gameObject) return;
        
        rg = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<RuntimeGraph>();

        if(propertyType != PropertyType.SceneReference){
            contextMenu.OpenAt(eventData.position, null);
            contextMenu.SetAction = CreateSetNode;
            contextMenu.GetAction = CreateGetNode;
        }
        else
        {
            CreateGetNode();
        }

        return;
    }

    private void CreateGetNode()
    {
        if(!rg) return;

        NTScrollRect ntScrollRect = rg.GetComponentInChildren<NTScrollRect>();

        if(ntScrollRect == null) return;

        Vector2 nodePosition = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform) ntScrollRect.content.transform , 
            Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out nodePosition
        ); 
        
        nodePosition = new Vector2( nodePosition.x - 80, -nodePosition.y);

        Node node = rg.graph.AddNode(typeof(GetNTVariableNode));
        node.position = nodePosition;
        node.name = "GET - (" + SessionManager.Instance.selectedSceneObject.name + ")";
        
        if(propertyType == PropertyType.SceneReference){
            ((GetNTVariableNode) node).SetVariableKey(SessionManager.Instance.selectedSceneObject.data.id, typeof(string), path, typeof(SceneGameObject) );
        }
        else
        {
            ((GetNTVariableNode) node).SetVariableKey(SessionManager.Instance.selectedSceneObject.data.id, typeof(string), path, data.GetType() );
        }

        rg.Refresh();
    }

    private void CreateSetNode()
    {
        if(!rg) return;

        NTScrollRect ntScrollRect = rg.GetComponentInChildren<NTScrollRect>();

        if(ntScrollRect == null) return;

        Vector2 nodePosition = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform) ntScrollRect.content.transform , 
            Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out nodePosition
        ); 
        
        nodePosition = new Vector2( nodePosition.x - 80, -nodePosition.y);

        string dataid = SessionManager.Instance.selectedSceneObject.data.id;

        Node node = rg.graph.AddNode(typeof(SetNTVariableNode));
        node.position = nodePosition;
        node.name = "SET - (" + SessionManager.Instance.selectedSceneObject.name + ")";

        if(propertyType == PropertyType.SceneReference){
            Debug.LogWarning("Not valid option");
            
            //FIXME: Avoid setting scene references as user can mess up things..
            //((SetNTVariableNode) node).SetVariableKey(SessionManager.Instance.selectedSceneObject.data.id, typeof(string), path, typeof(SceneGameObject) );
        }
        else
        {
            ((SetNTVariableNode) node).SetVariableKey(dataid, typeof(string), path, data.GetType() );
        }

        rg.Refresh();
    }
}