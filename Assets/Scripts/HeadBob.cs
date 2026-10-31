
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Referencias")]
    public CharacterController controller;

    [Header("Movimiento al caminar")]
    public float walkFrequency = 9f;
    public float walkAmplitude = 0.035f;

    [Header("Movimiento al correr")]
    public float runFrequency = 13f;
    public float runAmplitude = 0.065f;

    [Header("Suavizado")]
    public float smoothSpeed = 10f;

    private Vector3 originalPosition;
    private float bobTimer;

    void Start()
    {
        originalPosition = transform.localPosition;

        if (controller == null)
        {
            controller = GetComponentInParent<CharacterController>();
        }
    }

    void LateUpdate()
    {
        if (controller == null)
            return;

        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;

        bool isMoving = horizontalVelocity.magnitude > 0.15f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 targetPosition = originalPosition;

        if (controller.isGrounded && isMoving)
        {
            float frequency = isRunning ? runFrequency : walkFrequency;
            float amplitude = isRunning ? runAmplitude : walkAmplitude;

            bobTimer += Time.deltaTime * frequency;

            targetPosition.y += Mathf.Sin(bobTimer) * amplitude;
            targetPosition.x += Mathf.Cos(bobTimer * 0.5f) * amplitude * 0.5f;
        }
        else
        {
            bobTimer = 0f;
        }

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
