using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartSceneHighscore : MonoBehaviour
{
    public TextMeshProUGUI bestText;

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);
        float bestTime = PlayerPrefs.GetFloat("BEST_TIME", 0f);

        int m = Mathf.FloorToInt(bestTime / 60f);
        int s = Mathf.FloorToInt(bestTime % 60f);

        bestText.text = $"High Score: {highScore}   Best Time: {m:00}:{s:00}";
    }
}
