using UnityEngine;

public class EnemyTankStatic_att : MonoBehaviour
{
    [Header("Referencias")]
    public Transform turret;
    public Transform player;

    [Header("Configuración")]
    public float rangoVision = 15f;
    public float velocidadRotacion = 5f;

    public LayerMask obstacleMask;

    void Update()
    {
        if (player == null)
            return;

        Vector3 direccion =
            player.position - turret.position;

        float distancia = direccion.magnitude;

        if (distancia > rangoVision)
            return;

        direccion.y = 0;

        Ray ray = new Ray(
            turret.position,
            direccion.normalized
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            rangoVision,
            ~0))
        {
            if (hit.transform.CompareTag("Player"))
            {
                RotarTorreta(direccion);

                Debug.Log("Jugador detectado.");
            }
        }
    }

    void RotarTorreta(Vector3 direccion)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(direccion);
        targetRotation *= Quaternion.Euler(0, 180, 0);
        turret.rotation = Quaternion.Slerp(
            turret.rotation,
            targetRotation,
            velocidadRotacion * Time.deltaTime
        );
    }
}