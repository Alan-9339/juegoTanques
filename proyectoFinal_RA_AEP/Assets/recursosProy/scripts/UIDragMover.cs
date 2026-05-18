using UnityEngine;
using UnityEngine.EventSystems; // Obligatorio para detectar toques

public class UIDragMover : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Buscamos el Canvas padre para ajustar la escala del movimiento
        canvas = GetComponentInParent<Canvas>();
    }

    // Al iniciar el toque, guardamos donde tocó respecto al centro del objeto
    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out offset);
    }

    // Mientras arrastra, movemos el objeto
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Movemos la posición anclada basándonos en el movimiento del delta táctil
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}