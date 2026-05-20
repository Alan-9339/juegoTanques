using UnityEngine;

public class EnemyTankHunter : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform chassis;
    public Transform turret;
    public Transform visionPoint;

    [Header("Movimiento Base (Escala 1:1)")]
    public float moveSpeed;
    public float rotationSpeed = 5f;
    public float stopDistance; // Distancia deseada en escala normal (1)

    [Header("Combate Base (Escala 1:1)")]
    public float rangoVision = 20f;
    public LayerMask obstacleMask;

    private Rigidbody rb;
    private bool tieneLineaVision = false;

    // Variables internas que guardarán los rangos adaptados a la escala de la mesa/piso
    private float stopDistanceAdaptada;
    private float rangoVisionAdaptado;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null) player = jugador.transform;
        }

        // =====================================================
        // AJUSTAR ESCALA PARA REALIDAD AUMENTADA
        // =====================================================
        // Tomamos la escala del "mapaInstanciado" (el padre de este tanque) 
        // para reducir proporcionalmente los rangos de física y visión.
        float escalaMundoAR = transform.parent != null ? transform.parent.localScale.x : 1f;

        stopDistanceAdaptada = stopDistance * escalaMundoAR;
        rangoVisionAdaptado = rangoVision * escalaMundoAR;

        // Validar por si acaso la escala es extremadamente pequeña
        if (stopDistanceAdaptada <= 0.01f) stopDistanceAdaptada = 0.1f;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 direccionJugador = player.position - transform.position;
        direccionJugador.y = 0; // Mantener movimiento en plano horizontal
        float distancia = direccionJugador.magnitude;

        tieneLineaVision = VerificarLineaVision();

        // 1. LÓGICA DE LA TORRETA
        if (tieneLineaVision)
        {
            RotarTorreta(direccionJugador);
        }

        // 2. LÓGICA DEL CHASIS (Rotación)
        if (direccionJugador != Vector3.zero)
        {
            Quaternion targetChassisRotation = Quaternion.LookRotation(direccionJugador);
            chassis.rotation = Quaternion.Slerp(chassis.rotation, targetChassisRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // 3. LÓGICA DE MOVIMIENTO CON LA DISTANCIA ADAPTADA
        // Ahora comparamos contra la distancia escalada a la Realidad Aumentada
        if (distancia > stopDistanceAdaptada)
        {
            Vector3 movimiento = direccionJugador.normalized * moveSpeed * Time.fixedDeltaTime;
            
            // IMPORTANTE: También aplicamos MovePosition usando la velocidad física
            rb.MovePosition(rb.position + movimiento);
        }
        else
        {
            // Detener el tanque de forma segura si ya está en rango de disparo
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; 
        }
    }

    bool VerificarLineaVision()
    {
        if (visionPoint == null) return false;

        Vector3 origen = visionPoint.position;
        Vector3 direccionAlJugador = (player.position - origen).normalized;

        // Usamos el rango de visión adaptado
        if (Physics.Raycast(origen, direccionAlJugador, out RaycastHit hit, rangoVisionAdaptado, obstacleMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawRay(origen, direccionAlJugador * hit.distance, Color.red);
                return true;
            }
            else
            {
                Debug.DrawRay(origen, direccionAlJugador * hit.distance, Color.yellow);
            }
        }
        return false;
    }

    void RotarTorreta(Vector3 direccion)
    {
        if (direccion == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(direccion);
        Quaternion flatRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        turret.rotation = Quaternion.Slerp(turret.rotation, flatRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}