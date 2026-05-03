using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D hoverCursor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CustomCursor.Instance != null)
            CustomCursor.Instance.SetCursor(hoverCursor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CustomCursor.Instance != null)
            CustomCursor.Instance.SetDefaultCursor();
    }
}
