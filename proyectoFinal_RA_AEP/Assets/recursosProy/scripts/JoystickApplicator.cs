using UnityEngine;

public class JoystickApplicator : MonoBehaviour // <--- IMPORTANTE: añadir esto
{
    void Start() 
    {
        // El segundo número es el valor por defecto si no hay nada guardado
        float size = PlayerPrefs.GetFloat("JoystickSize", 1.0f);
        float posX = PlayerPrefs.GetFloat("JoystickPosX", 0f);

        RectTransform rt = GetComponent<RectTransform>();
        
        if (rt != null) 
        {
            rt.localScale = Vector3.one * size;
            // Ojo: Esto suma a la posición inicial. 
            // Si tu joystick ya está en X: 100 y posX es 50, terminará en 150.
            rt.anchoredPosition += new Vector2(posX, 0);
        }
    }
}