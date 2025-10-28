using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Image[] lifeIcons;
    public Button exitButton;

    private int score = 0;
    private float timeElapsed = 0f;
    private int lives = 3;

    void Start()
    {
        UpdateScore();
        UpdateLives();
        exitButton.onClick.AddListener(ReturnToMenu);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        timerText.text = "Time: " + Mathf.FloorToInt(timeElapsed).ToString();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScore();
    }

    public void LoseLife()
    {
        lives--;
        UpdateLives();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateLives()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].enabled = i < lives;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

}
