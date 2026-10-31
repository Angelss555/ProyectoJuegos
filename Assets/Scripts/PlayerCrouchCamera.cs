
using UnityEngine;

public class PlayerCrouchCamera : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Header("Altura de cámara")]
    public float standingCameraY = 0.7f;
    public float crouchingCameraY = 0.1f;
    public float transitionSpeed = 8f;

    private void LateUpdate()
    {
        if (playerMovement == null)
            return;

        float targetY = playerMovement.IsCrouching()
            ? crouchingCameraY
            : standingCameraY;

        Vector3 position = transform.localPosition;

        position.y = Mathf.MoveTowards(
            position.y,
            targetY,
            transitionSpeed * Time.deltaTime
        );

        transform.localPosition = position;
    }
}
