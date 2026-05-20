using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform chassis;
    public Transform turret;

    // Referencia al sistema de disparo
    public Proyectil1 sistemaDisparo;

    [Header("Joysticks")]
    public FixedJoystick moveJoystick;
    public FixedJoystick aimJoystick;

    [Header("Configuración")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 10f;
    public float turretRotationSpeed = 15f;

    private Rigidbody rb;
    private bool estabaApuntando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Detectar cuando el jugador suelta el joystick de apuntado
        if (aimJoystick != null)
        {
            float magnitude = new Vector2(
                aimJoystick.Horizontal,
                aimJoystick.Vertical
            ).magnitude;

            if (magnitude > 0.1f)
            {
                estabaApuntando = true;
            }
            else if (estabaApuntando && magnitude <= 0.1f)
            {
                estabaApuntando = false;
                Disparar();
            }
        }
    }

    void FixedUpdate()
    {
        if (moveJoystick != null)
            Move();

        if (aimJoystick != null)
            Aim();
    }

    void Move()
    {
        Vector3 input = new Vector3(
            moveJoystick.Horizontal,
            0,
            moveJoystick.Vertical
        );

        if (input.magnitude > 0.1f)
        {
            rb.MovePosition(
                rb.position +
                input.normalized *
                moveSpeed *
                Time.fixedDeltaTime
            );

            chassis.rotation = Quaternion.Slerp(
                chassis.rotation,
                Quaternion.LookRotation(input),
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    void Aim()
    {
        // Quitamos los negativos para evitar apuntado invertido
        Vector3 aimInput = new Vector3(-aimJoystick.Horizontal,0,-aimJoystick.Vertical);

        if (aimInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(aimInput);

            turret.rotation = Quaternion.Slerp(
                turret.rotation,
                targetRotation,
                turretRotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    void Disparar()
    {
        if (sistemaDisparo != null)
        {
            sistemaDisparo.Disparar();
        }
    }
}