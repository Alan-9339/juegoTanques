using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [Header("Referencias")]
    public Transform spawnPoint;
    public GameObject bullet;

    [Header("Configuración")]
    public float shotForce = 1500f;
    public float tiempoDeVida = 3f;

    public void Disparar()
    {
        if (bullet != null && spawnPoint != null)
        {
            // Crear bala
            GameObject newBullet = Instantiate(
                bullet,
                spawnPoint.position,
                spawnPoint.rotation
            );

            // Obtener Rigidbody
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();

            // Aplicar fuerza hacia adelante
            if (rb != null)
            {
                rb.AddForce(
                    spawnPoint.forward * shotForce
                );
            }

            // Destruir la bala después de unos segundos
            Destroy(newBullet, tiempoDeVida);

            // Debug visual
            Debug.DrawRay(
                spawnPoint.position,
                spawnPoint.forward * 2f,
                Color.red,
                3f
            );
        }
    }
}