using UnityEngine;

/// <summary>
/// Objeto recogible: leña, antorcha, botiquín, llave, etc.
/// Necesita un Collider marcado como "Is Trigger".
/// El jugador debe tener el tag "Player".
/// </summary>
public class Collectible : MonoBehaviour
{
    public enum CollectibleType { Score, Heal, WinItem }

    public CollectibleType type = CollectibleType.Score;

    [Tooltip("Puntos que suma (Score) o vida que cura (Heal)")]
    public int value = 10;

    [Header("Recoger con tecla")]
    [Tooltip("Si está marcado, hay que presionar la tecla estando cerca. Si no, se recoge al tocarlo.")]
    public bool requireKeyPress = true;
    public KeyCode interactKey = KeyCode.E;

    [Header("Efectos opcionales")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;
    [Tooltip("Hace que el objeto gire para que se note")]
    public bool rotate = false;

    private bool collected = false;
    private bool playerInRange = false;
    private GameObject playerObj;

    private void Update()
    {
        if (rotate) transform.Rotate(Vector3.up, 60f * Time.deltaTime, Space.World);

        if (requireKeyPress && playerInRange && Input.GetKeyDown(interactKey))
            Collect(playerObj);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerObj = other.gameObject;
        playerInRange = true;

        if (requireKeyPress) InteractionPrompt.Show($"Presiona {interactKey} para recoger");
        else Collect(playerObj);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        InteractionPrompt.Hide();
    }

    private void Collect(GameObject player)
    {
        if (collected) return;
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing) return;
        collected = true; // evita que se cuente dos veces

        switch (type)
        {
            case CollectibleType.Score:
                ScoreManager.Instance?.AddScore(value);
                break;
            case CollectibleType.Heal:
                player.GetComponent<Health>()?.Heal(value);
                break;
            case CollectibleType.WinItem:
                GameManager.Instance?.Win();
                break;
        }

        if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);

        InteractionPrompt.Hide();
        Destroy(gameObject);
    }
}
