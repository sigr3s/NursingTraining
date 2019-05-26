using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UnityExtensions
{
    public static void RunOnChildrenRecursive(this GameObject go, Action<GameObject> action) {
        if (go == null) return;
        foreach (var trans in go.GetComponentsInChildren<Transform>(true)) {
            action(trans.gameObject);
        }
    }

    public static T FindInParents<T>(this GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }

    public static void SetDraggedPosition(this GameObject m_DraggingIcon, PointerEventData data, RectTransform m_DraggingPlane ,bool dragOnSurfaces)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

}