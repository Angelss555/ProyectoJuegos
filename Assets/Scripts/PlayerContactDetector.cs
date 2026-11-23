using UnityEngine;

/// <summary>
/// Va en el Player (junto al CharacterController).
/// El CharacterController no genera OnCollisionEnter, así que este script usa
/// OnControllerColliderHit para avisarle a los DamageDealer con collider sólido.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerContactDetector : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        DamageDealer dealer = hit.collider.GetComponentInParent<DamageDealer>();
        if (dealer != null) dealer.TryDamage(gameObject);
    }
}
