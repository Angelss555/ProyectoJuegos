
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float acceleration = 12f;
    public float deceleration = 15f;

    [Header("Salto y gravedad")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Agacharse")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1.2f;
    public float crouchSmoothSpeed = 8f;
    public LayerMask obstructionMask = ~0;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 horizontalVelocity;

    private bool isCrouching;
    private float currentHeight;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHeight = controller.height;
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        // Agacharse manteniendo Ctrl
        bool wantsToCrouch = Input.GetKey(KeyCode.LeftControl);

        if (wantsToCrouch)
        {
            isCrouching = true;
        }
        else if (CanStandUp())
        {
            isCrouching = false;
        }

        // Cambiar altura del CharacterController
        float targetHeight = isCrouching
            ? crouchingHeight
            : standingHeight;

        currentHeight = Mathf.MoveTowards(
            currentHeight,
            targetHeight,
            crouchSmoothSpeed * Time.deltaTime
        );

        controller.height = currentHeight;

        // Mantener la parte inferior del collider en su sitio
        controller.center = new Vector3(
            0f,
            (currentHeight - standingHeight) * 0.5f,
            0f
        );

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction =
            transform.right * horizontal +
            transform.forward * vertical;

        direction = Vector3.ClampMagnitude(direction, 1f);

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        float targetSpeed = isCrouching
            ? crouchSpeed
            : (isRunning ? runSpeed : walkSpeed);

        Vector3 targetVelocity = direction * targetSpeed;

        float changeRate = direction.sqrMagnitude > 0.01f
            ? acceleration
            : deceleration;

        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            changeRate * Time.deltaTime
        );

        // No saltar estando agachado
        if (isGrounded && !isCrouching &&
            Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 movement =
            horizontalVelocity + Vector3.up * velocity.y;

        controller.Move(movement * Time.deltaTime);
    }

    bool CanStandUp()
    {
        float extraHeight = standingHeight - controller.height;

        if (extraHeight <= 0.01f)
            return true;

        Vector3 bottom = transform.position +
            controller.center -
            Vector3.up * (controller.height * 0.5f);

        Vector3 sphereStart = bottom +
            Vector3.up * (controller.radius + 0.05f);

        Vector3 sphereEnd = bottom +
            Vector3.up * (standingHeight - controller.radius);

        return !Physics.CheckCapsule(
            sphereStart,
            sphereEnd,
            controller.radius * 0.9f,
            obstructionMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }
}
