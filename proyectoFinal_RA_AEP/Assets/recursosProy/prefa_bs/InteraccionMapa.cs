using UnityEngine;

public class InteraccionMapa : MonoBehaviour
{
    private float velocidadGiro = 0.4f;
    private float velocidadEscala = 0.01f;

    void Update()
    {
        // ROTACIÓN: Con un dedo deslizándose horizontalmente
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            float deltaX = Input.GetTouch(0).deltaPosition.x;
            transform.Rotate(0, -deltaX * velocidadGiro, 0, Space.World);
        }

        // ESCALA: Con dos dedos (Pinch)
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 t0Prev = t0.position - t0.deltaPosition;
            Vector2 t1Prev = t1.position - t1.deltaPosition;

            float distPrev = (t0Prev - t1Prev).magnitude;
            float distActual = (t0.position - t1.position).magnitude;
            float diferencia = distActual - distPrev;

            Vector3 nuevaEscala = transform.localScale + Vector3.one * diferencia * velocidadEscala;
            
            // Límites para que no sea gigante o invisible
            nuevaEscala = Vector3.Max(nuevaEscala, new Vector3(0.05f, 0.05f, 0.05f)); 
            transform.localScale = nuevaEscala;
        }
    }
}