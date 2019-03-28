using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class NTScrollRect : ScrollRect
{

    [Header("Graph movement")]
    public float scrollSensivility = 0.06f;
    private float minScale = 0.2f;
    private float maxScale = 1.5f;


    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnBeginDrag(eventData);
        } 
    }
 
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnEndDrag(eventData);
        }    
    }
 
    public override void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnDrag(eventData);
        }
    }

    public override void OnScroll(PointerEventData data){
        
        Vector2 scrollDelta = data.scrollDelta;

        float newScale =  content.transform.localScale.x + scrollDelta.y * scrollSensivility;

        newScale = newScale < minScale ? minScale : newScale;
        newScale = newScale > maxScale ? maxScale : newScale;

        content.transform.localScale = new Vector3(newScale, newScale, newScale);


    }
}