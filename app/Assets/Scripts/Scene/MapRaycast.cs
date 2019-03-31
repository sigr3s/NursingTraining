using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(RectTransform))]
public class MapRaycast : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ICanvasRaycastFilter
{
    public bool shouldRaycastMap = false;

    RectTransform _rectTransform;
    RectTransform rectTransform{
        get{
            if(_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

            return _rectTransform;
        }
    }

    public Vector2 textureCoords;



    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        Vector2 pivotToCursorVector;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, sp, eventCamera, out pivotToCursorVector);

        float x = (pivotToCursorVector.x / rectTransform.rect.width ) + 0.5f;
        float y = (pivotToCursorVector.y / rectTransform.rect.height) + 0.5f;

        textureCoords = new Vector2(x,y);

        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shouldRaycastMap = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shouldRaycastMap = true;
    }
}