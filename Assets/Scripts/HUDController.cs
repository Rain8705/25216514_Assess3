using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Main")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Image[] lifeIcons;

    [Header("Level Info")]
    public TextMeshProUGUI levelNameText; // NEW — Level name display

    [Header("Overlays")]
    public CanvasGroup countdownBlock;
    public TextMeshProUGUI countdownText;
    public CanvasGroup gameOverBlock;

    [Header("Ghost Timer")]
    public CanvasGroup ghostTimerBlock;
    public TextMeshProUGUI ghostTimerText;

    int score = 0;
    int lives = 3;

    public int CurrentScore => score;

    void Start()
    {
        // Make sure optional UI starts hidden
        if (ghostTimerBlock)
        {
            ghostTimerBlock.alpha = 0f;
            ghostTimerBlock.blocksRaycasts = false;
            ghostTimerBlock.interactable = false;
        }

        if (gameOverBlock)
        {
            gameOverBlock.alpha = 0f;
            gameOverBlock.blocksRaycasts = false;
            gameOverBlock.interactable = false;
        }

        if (countdownBlock)
        {
            countdownBlock.alpha = 0f;
            countdownBlock.blocksRaycasts = false;
            countdownBlock.interactable = false;
        }

        UpdateScoreText();
        UpdateLivesUI();
    }

    // ---------------- TIMER ----------------
    public void SetTimer(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        int cs = Mathf.FloorToInt((t - Mathf.Floor(t)) * 100f);
        if (timerText)
            timerText.text = $"{m:00}:{s:00}:{cs:00}";
    }

    // ---------------- SCORE ----------------
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText)
            scoreText.text = $"Score: {score}";
    }

    // ---------------- LIVES ----------------
    public void LoseLife()
    {
        lives = Mathf.Max(0, lives - 1);
        UpdateLivesUI();

        if (lives <= 0 && GameManager.I != null)
            GameManager.I.OnLivesDepleted();
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            if (lifeIcons[i]) lifeIcons[i].enabled = i < lives;
    }

    // ---------------- LEVEL NAME ----------------
    public void SetLevelName(string level)
    {
        if (levelNameText)
            levelNameText.text = level;
    }

    // ---------------- COUNTDOWN ----------------
    public void ShowCountdownBlock(bool on)
    {
        if (!countdownBlock) return;
        countdownBlock.alpha = on ? 1f : 0f;
        countdownBlock.blocksRaycasts = on;
        countdownBlock.interactable = on;
    }

    public void SetCountdown(string s)
    {
        if (countdownText)
            countdownText.text = s;
    }

    // ---------------- GAME OVER ----------------
    public void ShowGameOverBlock(bool on)
    {
        if (!gameOverBlock) return;
        gameOverBlock.alpha = on ? 1f : 0f;
        gameOverBlock.blocksRaycasts = on;
        gameOverBlock.interactable = on;
    }

    // ---------------- GHOST TIMER ----------------
    public void ShowGhostTimer(bool on)
    {
        if (!ghostTimerBlock) return;
        ghostTimerBlock.alpha = on ? 1f : 0f;
        ghostTimerBlock.blocksRaycasts = on;
        ghostTimerBlock.interactable = on;
    }

    public void SetGhostTimer(float secondsLeft)
    {
        if (ghostTimerText)
            ghostTimerText.text = Mathf.CeilToInt(secondsLeft).ToString();
    }

    // ---------------- MENU BUTTON ----------------
    public void ExitToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}
