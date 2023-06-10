using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseOver;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}