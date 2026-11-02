using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Controla el estado de la partida: jugando, ganó o perdió.
/// Va en un GameObject vacío llamado "GameManager" (uno solo por escena).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Won, Lost }
    public GameState State { get; private set; } = GameState.Playing;

    [Header("Jugador (si se deja vacío, busca el objeto con tag Player)")]
    public GameObject player;

    [Header("Condiciones de victoria (activen las que diga el GDD)")]
    [Tooltip("Ganar al llegar a cierto puntaje")]
    public bool winByScore = true;
    public int scoreToWin = 100;

    [Tooltip("Ganar al eliminar a todos los enemigos (tag 'Enemy')")]
    public bool winByKillingAllEnemies = false;

    [Tooltip("Ganar al sobrevivir cierto tiempo")]
    public bool winBySurvivingTime = false;
    public float timeToSurvive = 60f;

    [Header("Condiciones de derrota")]
    [Tooltip("Perder si se acaba el tiempo (0 = sin límite)")]
    public float timeLimit = 0f;

    [Tooltip("Perder si el jugador cae por debajo de esta altura (se sale del terreno)")]
    public float fallLimitY = -20f;

    public float ElapsedTime { get; private set; }

    public event Action OnWin;
    public event Action OnLose;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Time.timeScale = 1f;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged += CheckScoreWin;
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= CheckScoreWin;
    }

    private void Update()
    {
        if (State != GameState.Playing) return;

        ElapsedTime += Time.deltaTime;

        if (player != null && player.transform.position.y < fallLimitY) { Lose(); return; }

        if (winBySurvivingTime && ElapsedTime >= timeToSurvive) Win();
        else if (timeLimit > 0 && ElapsedTime >= timeLimit) Lose();
    }

    private void CheckScoreWin(int score)
    {
        if (winByScore && score >= scoreToWin) Win();
    }

    /// <summary>Lo llama Health cuando muere un enemigo.</summary>
    public void NotifyEnemyKilled()
    {
        if (!winByKillingAllEnemies || State != GameState.Playing) return;
        Invoke(nameof(CheckEnemiesLeft), 0.1f); // espera a que el enemigo se destruya
    }

    private void CheckEnemiesLeft()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) Win();
    }

    public void Win()
    {
        if (State != GameState.Playing) return;
        State = GameState.Won;
        Debug.Log("¡GANASTE!");
        EndGame();
        OnWin?.Invoke();
    }

    public void Lose()
    {
        if (State != GameState.Playing) return;
        State = GameState.Lost;
        Debug.Log("PERDISTE");
        EndGame();
        OnLose?.Invoke();
    }

    /// <summary>
    /// Congela al jugador y libera el mouse para poder hacer clic en los botones.
    /// Es necesario porque MouseLook vuelve a bloquear el cursor al hacer clic.
    /// </summary>
    private void EndGame()
    {
        if (player != null)
        {
            var movement = player.GetComponent<PlayerMovement>();
            if (movement) movement.enabled = false;

            foreach (var look in player.GetComponentsInChildren<MouseLook>())
                look.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    // ---- Botones ----
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
