using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GhostState { Normal, Scared, Recovering, Dead }

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Refs")]
    public PacStudentController pac;
    public List<GhostController> ghosts = new List<GhostController>();
    public HUDController hud;
    public AudioManager audioMgr;

    [Header("Round")]
    public float countdownDelay = 1f;
    public bool canPlayerMove = false;
    public bool canGhostsMove = false;

    [Header("Timers")]
    public float gameTime = 0f;
    bool timeRunning = false;

    [Header("Scared Mode")]
    public float scaredDuration = 10f;
    float scaredLeft = 0f;

    [Header("Scoring")]
    public int pelletsRemaining = 0;

    const string KEY_HIGH = "HIGH_SCORE";
    const string KEY_BEST_TIME = "BEST_TIME";

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // ✅ FIX: count pellets in the level before the round starts
        pelletsRemaining = GameObject.FindGameObjectsWithTag("Pellet").Length;
        Debug.Log($"Pellets found in scene: {pelletsRemaining}");

        StartCoroutine(RoundStart());
    }

    void Update()
    {
        if (timeRunning)
        {
            gameTime += Time.deltaTime;
            if (hud) hud.SetTimer(gameTime);
        }

        if (scaredLeft > 0f)
        {
            scaredLeft -= Time.deltaTime;
            if (hud)
            {
                hud.ShowGhostTimer(true);
                hud.SetGhostTimer(Mathf.Max(0f, scaredLeft));
            }

            if (scaredLeft <= 3f) BroadcastGhostState(GhostState.Recovering);
            if (scaredLeft <= 0f) EndScaredMode();
        }
    }

    IEnumerator RoundStart()
    {
        canPlayerMove = false;
        canGhostsMove = false;
        timeRunning = false;

        if (hud) hud.ShowCountdownBlock(true);

        string[] seq = { "3", "2", "1", "GO!" };
        foreach (var s in seq)
        {
            if (hud) hud.SetCountdown(s);
            yield return new WaitForSeconds(countdownDelay);
        }

        if (hud) hud.ShowCountdownBlock(false);

        canPlayerMove = true;
        if (pac) pac.AwaitInput();

        canGhostsMove = true;
        timeRunning = true;

        if (audioMgr) audioMgr.PlayBGM(audioMgr.bgmNormal, true);
    }

    public void StartScaredMode()
    {
        scaredLeft = scaredDuration;
        BroadcastGhostState(GhostState.Scared);
        if (audioMgr) audioMgr.PlayBGM(audioMgr.bgmScared, true);
    }

    void EndScaredMode()
    {
        if (hud) hud.ShowGhostTimer(false);
        BroadcastGhostState(GhostState.Normal);
        if (audioMgr) audioMgr.PlayBGM(audioMgr.bgmNormal, true);
    }

    void BroadcastGhostState(GhostState s)
    {
        for (int i = 0; i < ghosts.Count; i++)
        {
            var g = ghosts[i];
            if (g) g.SetState(s);
        }
    }

    public void OnPacDied()
    {
        StartCoroutine(CoPacDeath());
    }

    IEnumerator CoPacDeath()
    {
        canPlayerMove = false;
        canGhostsMove = false;
        timeRunning = false;

        if (audioMgr) audioMgr.PlaySFX(audioMgr.sfxDeath);
        if (pac) pac.PlayDeathParticles();

        yield return new WaitForSeconds(1.5f);

        if (pac) pac.Respawn();

        // Reset all ghosts
        foreach (var g in ghosts)
        {
            if (g) g.ResetGhost();
        }

        EndScaredMode();

        canPlayerMove = true;
        if (pac) pac.AwaitInput();

        canGhostsMove = true;
        timeRunning = true;

        if (audioMgr) audioMgr.PlayBGM(audioMgr.bgmNormal, true);
    }

    public void OnGhostEaten(GhostController ghost)
    {
        if (hud) hud.AddScore(300);
        StartCoroutine(CoGhostRecover(ghost));
    }

    IEnumerator CoGhostRecover(GhostController ghost)
    {
        if (audioMgr) audioMgr.PlayBGM(audioMgr.bgmDead, false);
        yield return new WaitForSeconds(3f);

        if (ghost)
        {
            if (scaredLeft > 3f) ghost.SetState(GhostState.Scared);
            else if (scaredLeft > 0f) ghost.SetState(GhostState.Recovering);
            else ghost.SetState(GhostState.Normal);
        }

        if (audioMgr && scaredLeft <= 0f) audioMgr.PlayBGM(audioMgr.bgmNormal, true);
    }

    // ✅ Only trigger GameOver when pellets actually reach 0
    public void OnAllPelletsEaten()
    {
        if (pelletsRemaining <= 0)
        {
            Debug.Log("All pellets eaten — game over triggered");
            GameOver();
        }
    }

    public void OnLivesDepleted() { GameOver(); }

    public void GameOver()
    {
        canPlayerMove = false;
        canGhostsMove = false;
        timeRunning = false;

        if (hud) hud.ShowGameOverBlock(true);

        int score = hud ? hud.CurrentScore : 0;
        int bestScore = PlayerPrefs.GetInt(KEY_HIGH, 0);
        float bestTime = PlayerPrefs.GetFloat(KEY_BEST_TIME, float.MaxValue);

        bool better = score > bestScore || (score == bestScore && gameTime < bestTime);
        if (better)
        {
            PlayerPrefs.SetInt(KEY_HIGH, score);
            PlayerPrefs.SetFloat(KEY_BEST_TIME, gameTime);
            PlayerPrefs.Save();
        }

        StartCoroutine(ReturnToMenu());
    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("StartScene");
    }

    public static (int high, float time) LoadRecord()
    {
        return (PlayerPrefs.GetInt(KEY_HIGH, 0), PlayerPrefs.GetFloat(KEY_BEST_TIME, 0f));
    }
}
