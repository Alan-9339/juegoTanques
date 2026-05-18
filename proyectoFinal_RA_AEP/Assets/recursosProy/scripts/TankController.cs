using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform chassis;
    public Transform turret;

    [Header("Joysticks")]
    public FixedJoystick moveJoystick;
    public FixedJoystick aimJoystick;

    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 10f;
    public float turretRotationSpeed = 15f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            Vector3 movimiento =
                input.normalized *
                moveSpeed *
                Time.fixedDeltaTime;

            rb.MovePosition(
                rb.position + movimiento
            );

            Quaternion targetRotation =
                Quaternion.LookRotation(input);

            chassis.rotation = Quaternion.Slerp(
                chassis.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    void Aim()
    {
        float horizontal =
            -aimJoystick.Horizontal;

        float vertical =
            -aimJoystick.Vertical;

        Vector3 aimInput = new Vector3(
            horizontal,
            0,
            vertical
        );

        if (aimInput.magnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(aimInput);

            Quaternion flatRotation =
                Quaternion.Euler(
                    0,
                    targetRotation.eulerAngles.y,
                    0
                );

            turret.rotation = Quaternion.Slerp(
                turret.rotation,
                flatRotation,
                turretRotationSpeed * Time.fixedDeltaTime
            );
        }
    }
}