using UnityEngine;

public class JoystickPersistencePro : MonoBehaviour
{
    // Usaremos un prefijo para distinguir Joy Izquierdo de Derecho
    // Escribe "LeftJoy" o "RightJoy" en el Inspector para cada instancia
    [SerializeField] private string joystickPrefix; 

    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError($"[JoystickPersistencePro] {gameObject.name} no tiene RectTransform.");
            return;
        }

        // 1. Cargamos Posición X e Y (usando la actual como valor por defecto si no hay guardado)
        float savedX = PlayerPrefs.GetFloat(joystickPrefix + "PosX", rt.anchoredPosition.x);
        float savedY = PlayerPrefs.GetFloat(joystickPrefix + "PosY", rt.anchoredPosition.y);

        // 2. Cargamos Escala (usando 1.0 como defecto)
        float savedScale = PlayerPrefs.GetFloat(joystickPrefix + "Scale", 1.0f);

        // 3. Aplicamos los cambios al RectTransform
        rt.anchoredPosition = new Vector2(savedX, savedY);
        rt.localScale = Vector3.one * savedScale;
        
        Debug.Log($"[JoystickPersistencePro] {joystickPrefix} cargado: Pos({savedX}, {savedY}), Escala({savedScale})");
    }
}