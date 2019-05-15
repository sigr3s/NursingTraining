using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;

public class NTScrollRect : ScrollRect
{

    [Header("Graph movement")]
    public float scrollSensivility = 0.06f;
    private float minScale = 0.2f;
    private float maxScale = 1.5f;

    public Vector2 mouseStartPosition  = Vector2.zero;
    public Vector2 mouseEndPosition  = Vector2.zero;
    public Rect selectionRect;
    public RectTransform selectionGameObject;


    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnBeginDrag(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(selectionGameObject == null){
                selectionGameObject = (RectTransform) content.Find("SelectionRect");
            }

            selectionGameObject.gameObject.SetActive(true);
             
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform) content.transform , 
                Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out mouseStartPosition
            ); 
        } 
    }
 
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnEndDrag(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform) content.transform , 
                Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out mouseEndPosition
            ); 

            Vector2 size = mouseStartPosition - mouseEndPosition;
            Vector2 startPosition = mouseStartPosition - size/2;

            if (size.x < 0) { size.x = Mathf.Abs(size.x); }
            if (size.y < 0) { size.y = Mathf.Abs(size.y); }

            selectionRect = new Rect(startPosition, size);
            
            selectionGameObject.localPosition = selectionRect.position;
            selectionGameObject.sizeDelta = selectionRect.size;

            selectionGameObject.SetAsLastSibling();
            selectionGameObject.ForceUpdateRectTransforms();

            UGUIBaseNode[] nodes =  GetComponentsInChildren<UGUIBaseNode>();
            RuntimeGraph rtg = GetComponentInParent<RuntimeGraph>();
            rtg.selectedNodes = new List<IUGUINode>();
            
            foreach (var item in nodes)
            {
                RectTransform nodeTransform = (RectTransform) item.transform;
                Vector2 nodePosition = nodeTransform.localPosition;
                nodePosition += new Vector2(nodeTransform.sizeDelta.x/2f, -nodeTransform.sizeDelta.y/2f);
                
                if( nodePosition.x > (selectionRect.position.x - selectionRect.size.x/2f) &&  
                    nodePosition.x < (selectionRect.position.x + selectionRect.size.x/2f) &&
                    nodePosition.y > (selectionRect.position.y - selectionRect.size.y/2f) && 
                    nodePosition.y < (selectionRect.position.y + selectionRect.size.y/2f)  ){
                    
                    item.GetComponent<Image>().color = Color.yellow;
                    rtg.selectedNodes.Add(item);
                }
                else
                {
                    item.SetColor();
                }
            }

            selectionGameObject.gameObject.SetActive(false); 
        }    
    }
 
    public override void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.button = PointerEventData.InputButton.Left;
            base.OnDrag(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(selectionGameObject == null){
               selectionGameObject = (RectTransform) content.Find("SelectionRect");
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform) content.transform , 
                Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out mouseEndPosition
            ); 

            Vector2 size = mouseStartPosition - mouseEndPosition;
            Vector2 startPosition = mouseStartPosition - size/2;

            if (size.x < 0) { size.x = Mathf.Abs(size.x); }
            if (size.y < 0) { size.y = Mathf.Abs(size.y); }
            
            selectionRect = new Rect(startPosition, size);

            selectionGameObject.localPosition = selectionRect.position;
            selectionGameObject.sizeDelta = selectionRect.size;
            selectionGameObject.SetAsLastSibling();
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