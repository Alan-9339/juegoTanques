using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public int damage = 1;
    
    // Etiqueta del tanque que disparó para no hacerse daño a sí mismo
    public string tagTirador = "Player"; 

    void OnCollisionEnter(Collision collision)
    {
        // 1. Evitar que la bala choque con el cañón del propio jugador apenas nace
        if (collision.gameObject.CompareTag(tagTirador))
        {
            return;
        }

        // 2. Verificar si chocamos contra un enemigo
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Buscamos el script de salud en el enemigo impactado
            EnemyHealth health = collision.gameObject.GetComponent<EnemyHealth>();
            
            if (health != null)
            {
                health.RecibirDano(damage);
            }
        }

        // 3. La bala se destruye al chocar con cualquier cosa (muros, suelo, enemigos)
        Destroy(gameObject);
    }
}