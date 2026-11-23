using UnityEngine;
using System;

/// <summary>
/// Sistema de salud reutilizable para el jugador Y para los enemigos.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    [Tooltip("Márquenlo SOLO en el Player: si muere, se pierde la partida")]
    public bool isPlayer = false;

    [Header("Invulnerabilidad tras recibir daño")]
    [Tooltip("Segundos sin recibir daño después de un golpe")]
    public float invulnerabilityTime = 1f;
    private float lastHitTime = -999f;

    [Header("Al morir (enemigos)")]
    public int scoreOnDeath = 0;
    public bool destroyOnDeath = true;

    [Header("Sonidos opcionales")]
    public AudioClip hurtSound;
    public AudioClip deathSound;

    public bool IsDead { get; private set; }

    public event Action<int, int> OnHealthChanged; // (actual, máxima)
    public event Action OnDamaged;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0) return;
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing) return;
        if (Time.time - lastHitTime < invulnerabilityTime) return;

        lastHitTime = Time.time;
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnDamaged?.Invoke();

        if (CurrentHealth == 0) Die();
        else if (hurtSound) AudioSource.PlayClipAtPoint(hurtSound, transform.position);
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        if (deathSound) AudioSource.PlayClipAtPoint(deathSound, transform.position);
        OnDeath?.Invoke();

        if (isPlayer)
        {
            GameManager.Instance?.Lose();
            return; // al jugador no se le destruye, solo termina la partida
        }

        if (scoreOnDeath > 0) ScoreManager.Instance?.AddScore(scoreOnDeath);
        GameManager.Instance?.NotifyEnemyKilled();

        if (destroyOnDeath) Destroy(gameObject);
    }
}
