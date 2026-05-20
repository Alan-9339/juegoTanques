using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 1;

    private int currentHealth;

    private ControladorAR controladorAR;

    void Start()
    {
        currentHealth = maxHealth;

        // Buscar el controlador principal
        controladorAR = FindObjectOfType<ControladorAR>();
    }

    public void RecibirDano(int cantidad)
    {
        currentHealth -= cantidad;

        if (currentHealth <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("¡Enemigo destruido!");

        // Avisar al controlador
        if (controladorAR != null)
        {
            controladorAR.EnemigoEliminado();
        }

        Destroy(gameObject);
    }
}