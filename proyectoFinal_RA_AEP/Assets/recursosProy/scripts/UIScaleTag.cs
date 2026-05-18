using UnityEngine;
using UnityEngine.EventSystems; // Asegúrate de que esta línea esté presente

// AQUÍ ESTÁ EL CAMBIO: Debes agregar IBeginDragHandler después de la coma
public class UIScaleTag : MonoBehaviour, IDragHandler, IBeginDragHandler 
{
    [SerializeField] private RectTransform parentJoystick;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.8f;

    private float initialDistance;
    private Vector3 initialScale;

    // Ahora Unity ya no se quejará de esta función:
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentJoystick == null) return;
        
        initialDistance = Vector2.Distance(parentJoystick.position, transform.position);
        initialScale = parentJoystick.localScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentJoystick == null || initialDistance == 0) return;

        float currentDistance = Vector2.Distance(parentJoystick.position, eventData.position);
        float scaleFactor = currentDistance / initialDistance;

        Vector3 newScale = initialScale * scaleFactor;

        newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
        newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
        newScale.z = 1f;

        parentJoystick.localScale = newScale;
    }
}