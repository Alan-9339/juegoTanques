using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform chassis;     // Base del tanque
    public Transform turret;      // Torreta

    [Header("Joysticks")]
    public FixedJoystick moveJoystick; // Izquierdo
    public FixedJoystick aimJoystick;  // Derecho

    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 10f;
    public float turretRotationSpeed = 15f;

    void Update()
    {
        Move();
        Aim();
    }

    void Move()
    {
        // Input del joystick izquierdo
        Vector3 input = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical);

        if (input.magnitude > 0.1f)
        {
            // Movimiento en espacio global (IMPORTANTE para AR)
            transform.Translate(input * moveSpeed * Time.deltaTime, Space.World);

            // Rotación del chasis hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(input);

            chassis.rotation = Quaternion.Slerp(
                chassis.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void Aim()
    {
        // Input del joystick derecho
        Vector3 aimInput = new Vector3(aimJoystick.Horizontal, 0, aimJoystick.Vertical);

        if (aimInput.magnitude > 0.1f)
        {
            // Evitar inclinaciones raras
            aimInput.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(aimInput);

            // SOLO rotamos en Y (horizontal)
            Quaternion flatRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            turret.rotation = Quaternion.Slerp(
                turret.rotation,
                flatRotation,
                turretRotationSpeed * Time.deltaTime
            );
        }
    }
}