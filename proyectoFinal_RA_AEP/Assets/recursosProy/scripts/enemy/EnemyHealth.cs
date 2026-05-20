using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 1; // Soporta 1 disparo como solicitaste
    private int currentHealth;

    void Start()
    {
        // Al instanciarse el tanque, inicia con la salud máxima
        currentHealth = maxHealth;
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
        // Destruye el objeto del enemigo de la escena
        Debug.Log("¡Enemigo destruido!");
        Destroy(gameObject);
        
        // Opcional a futuro: Aquí puedes instanciar un Prefab de explosión
    }
}