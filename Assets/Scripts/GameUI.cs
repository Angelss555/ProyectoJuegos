using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Interfaz sencilla: vida, puntaje, tiempo, destello rojo al recibir daño
/// y paneles de ganar/perder. Va en el Canvas. Todos los campos son opcionales.
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("Referencias (si se deja vacío, busca el Player)")]
    public Health playerHealth;

    [Header("Salud")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    [Tooltip("Image roja que cubre toda la pantalla con alpha 0")]
    public Image damageFlash;
    public float flashAlpha = 0.35f;
    public float flashFadeSpeed = 2f;

    [Header("Puntaje y tiempo")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI timerText;

    [Header("Paneles de fin de partida")]
    public GameObject winPanel;
    public GameObject losePanel;
    public TextMeshProUGUI finalScoreWinText;
    public TextMeshProUGUI finalScoreLoseText;

    private void Start()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        SetFlashAlpha(0f);

        if (playerHealth == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerHealth = p.GetComponent<Health>();
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealth;
            playerHealth.OnDamaged += Flash;
            UpdateHealth(playerHealth.CurrentHealth, playerHealth.maxHealth);
        }
        else Debug.LogWarning("GameUI: no se encontró Health en el Player.");

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScore;
            UpdateScore(ScoreManager.Instance.Score);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWin += ShowWin;
            GameManager.Instance.OnLose += ShowLose;
        }
        else Debug.LogWarning("GameUI: falta el GameManager en la escena.");
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealth;
            playerHealth.OnDamaged -= Flash;
        }
        if (ScoreManager.Instance != null) ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWin -= ShowWin;
            GameManager.Instance.OnLose -= ShowLose;
        }
    }

    private void Update()
    {
        // Desvanecer el destello rojo (unscaled para que funcione aunque el juego esté pausado)
        if (damageFlash && damageFlash.color.a > 0f)
            SetFlashAlpha(Mathf.MoveTowards(damageFlash.color.a, 0f, flashFadeSpeed * Time.unscaledDeltaTime));

        if (timerText && GameManager.Instance != null)
        {
            var gm = GameManager.Instance;
            float t = gm.timeLimit > 0 ? Mathf.Max(gm.timeLimit - gm.ElapsedTime, 0) : gm.ElapsedTime;
            timerText.text = $"Tiempo: {Mathf.FloorToInt(t / 60):00}:{Mathf.FloorToInt(t % 60):00}";
        }
    }

    private void UpdateHealth(int current, int max)
    {
        if (healthBar)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }
        if (healthText) healthText.text = $"Vida: {current}/{max}";
    }

    private void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = $"Puntos: {score}";
        if (highScoreText && ScoreManager.Instance != null)
            highScoreText.text = $"Récord: {ScoreManager.Instance.HighScore}";
    }

    private void Flash() => SetFlashAlpha(flashAlpha);

    private void SetFlashAlpha(float a)
    {
        if (!damageFlash) return;
        Color c = damageFlash.color;
        c.a = a;
        damageFlash.color = c;
    }

    private void ShowWin()
    {
        if (winPanel) winPanel.SetActive(true);
        if (finalScoreWinText) finalScoreWinText.text = $"Puntaje final: {CurrentScore()}";
    }

    private void ShowLose()
    {
        if (losePanel) losePanel.SetActive(true);
        if (finalScoreLoseText) finalScoreLoseText.text = $"Puntaje final: {CurrentScore()}";
    }

    private int CurrentScore() => ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;

    // Conectar a los OnClick de los botones
    public void OnRestartButton() => GameManager.Instance?.RestartLevel();
    public void OnNextLevelButton() => GameManager.Instance?.LoadNextLevel();
    public void OnQuitButton() => GameManager.Instance?.QuitGame();
}
