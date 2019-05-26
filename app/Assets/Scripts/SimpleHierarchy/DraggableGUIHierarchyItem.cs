
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableGUIHierarchyItem : GUIHierarchyItem, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [Header("Drag")]
    public bool dragOnSurfaces = true;
    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    protected RuntimeGraph rg;
    protected Vector2 nodePosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!CanBeDragged()) return;
        
        var canvas = gameObject.FindInParents<Canvas>();
        if (canvas == null)
            return;

        m_DraggingIcon = new GameObject("icon");

        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();

        image.sprite = GetComponentInChildren<Image>().sprite;
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

        if(!rg) return;

        NTScrollRect ntScrollRect = rg.GetComponentInChildren<NTScrollRect>();

        if(ntScrollRect == null) return;

        nodePosition = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform) ntScrollRect.content.transform , 
            Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out nodePosition
        );

        nodePosition = new Vector2( nodePosition.x - 80, -nodePosition.y);

        OnDragEnd(eventData);

        return;
    }

    public virtual bool CanBeDragged(){
        return true;
    }

    public virtual void OnDragEnd(PointerEventData eventData){
        
    }

}