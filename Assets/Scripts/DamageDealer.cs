using UnityEngine;

/// <summary>
/// Hace daño al tocar: enemigos, fogata, pinchos, rocas que caen, proyectiles...
///
/// IMPORTANTE: el jugador usa un CharacterController, que NO dispara OnCollisionEnter.
/// Por eso este script funciona con:
///   1) Triggers (Collider con "Is Trigger") — lo recomendado.
///   2) Colliders sólidos, gracias a PlayerContactDetector (va en el jugador).
/// </summary>
public class DamageDealer : MonoBehaviour
{
    public int damage = 10;

    [Tooltip("Tag del objetivo (Player para enemigos/trampas, Enemy para ataques del jugador)")]
    public string targetTag = "Player";

    [Tooltip("Sigue haciendo daño mientras se esté tocando (fuego, pinchos)")]
    public bool damageWhileTouching = true;

    [Tooltip("Se destruye al golpear (proyectiles)")]
    public bool destroyOnHit = false;

    [Tooltip("Mata al instante")]
    public bool instantKill = false;

    private void OnTriggerEnter(Collider other) { TryDamage(other.gameObject); }
    private void OnTriggerStay(Collider other) { if (damageWhileTouching) TryDamage(other.gameObject); }

    // Para objetos con Rigidbody que choquen contra enemigos u otros objetos
    private void OnCollisionEnter(Collision c) { TryDamage(c.gameObject); }
    private void OnCollisionStay(Collision c) { if (damageWhileTouching) TryDamage(c.gameObject); }

    /// <summary>Público para que PlayerContactDetector lo pueda llamar.</summary>
    public void TryDamage(GameObject other)
    {
        if (!other.CompareTag(targetTag)) return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null) return;

        // La invulnerabilidad de Health evita que el daño continuo baje la vida en cada frame
        health.TakeDamage(instantKill ? health.maxHealth : damage);

        if (destroyOnHit) Destroy(gameObject);
    }
}
