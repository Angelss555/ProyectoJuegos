using UnityEngine;
using System;

/// <summary>
/// Lleva el puntaje de la partida. Uno solo por escena
/// (puede ir en el mismo GameObject que el GameManager).
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public int HighScore { get; private set; }

    public event Action<int> OnScoreChanged;

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    private void Start()
    {
        OnScoreChanged?.Invoke(Score);
    }

    public void AddScore(int amount)
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing) return;

        Score = Mathf.Max(Score + amount, 0); // nunca negativo
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
        }
        OnScoreChanged?.Invoke(Score);
    }
}
