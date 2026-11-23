using UnityEngine;

/// <summary>
/// Zona de meta (entrada de la cueva, salida, campamento) o zona de muerte (precipicio, agua).
/// Necesita un Collider con "Is Trigger" (por ejemplo, un Box Collider invisible).
/// </summary>
public class GoalZone : MonoBehaviour
{
    public enum ZoneType { Win, Lose }
    public ZoneType zoneType = ZoneType.Win;

    [Tooltip("Puntaje mínimo para que la meta funcione (0 = sin requisito)")]
    public int requiredScore = 0;

    [Tooltip("Mensaje si todavía no tiene el puntaje necesario")]
    public string lockedMessage = "Aún te faltan objetos por recoger";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || GameManager.Instance == null) return;

        if (zoneType == ZoneType.Lose)
        {
            GameManager.Instance.Lose();
            return;
        }

        int score = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;
        if (score >= requiredScore)
        {
            GameManager.Instance.Win();
        }
        else
        {
            InteractionPrompt.Show($"{lockedMessage} ({score}/{requiredScore})");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) InteractionPrompt.Hide();
    }
}
