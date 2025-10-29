using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public LayerMask wallLayer;
    public Transform pacStudent;

    private Vector2 direction;
    private Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private Rigidbody2D rb;
    private AudioManager audioManager;
    private Animator animator;
    private Vector3 startPosition;
    private bool isPaused;

    public GhostState currentState = GhostState.Normal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioManager = FindFirstObjectByType<AudioManager>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        if (pacStudent == null)
            pacStudent = GameObject.Find("PacStudent")?.transform;

        ChooseRandomDirection();
    }

    void FixedUpdate()
    {
        if (isPaused || currentState == GhostState.Dead)
            return;

        Move();
    }

    private void Move()
    {
        Vector2 nextPos = (Vector2)transform.position + direction * moveSpeed * Time.fixedDeltaTime;

        // Wall detection
        if (Physics2D.OverlapCircle(nextPos, 0.2f, wallLayer))
        {
            ChooseRandomDirection();
            return;
        }

        rb.MovePosition(nextPos);

        // ✅ Only flip horizontally (left/right)
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void ChooseRandomDirection()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 newDir = possibleDirections[Random.Range(0, possibleDirections.Length)];
            Vector2 checkPos = (Vector2)transform.position + newDir * 0.5f;

            if (!Physics2D.OverlapCircle(checkPos, 0.2f, wallLayer))
            {
                direction = newDir;
                return;
            }
        }
        direction = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentState == GhostState.Normal)
            {
                Debug.Log("Ghost hit PacStudent!");

                if (audioManager != null && audioManager.sfxDeath != null)
                    audioManager.PlaySFX(audioManager.sfxDeath);

                HUDController hud = FindFirstObjectByType<HUDController>();
                if (hud != null)
                    hud.LoseLife();

                if (pacStudent != null)
                    pacStudent.position = new Vector3(-3, -3, 0); // reset to spawn

                // 🔧 FIXED LINE BELOW
                GameManager.I?.OnPacDied();
            }
            else if (currentState == GhostState.Scared || currentState == GhostState.Recovering)
            {
                Debug.Log("Ghost eaten!");
                Die();
            }
        }    
    }

    private void Die()
    {
        currentState = GhostState.Dead;

        if (audioManager != null && audioManager.sfxDeath != null)
            audioManager.PlaySFX(audioManager.sfxDeath);

        if (animator != null)
            animator.Play("Ghost_Dead");

        StartCoroutine(RespawnAfterDelay(3f));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Respawn();
    }

    public void Respawn()
    {
        transform.position = startPosition;
        ChooseRandomDirection();
        currentState = GhostState.Normal;

        if (animator != null)
            animator.Play("Ghost_Walk");
    }

    public void SetState(GhostState newState)
    {
        currentState = newState;

        if (animator == null)
            return;

        switch (newState)
        {
            case GhostState.Normal:
                animator.Play("Ghost_Walk");
                break;
            case GhostState.Scared:
                animator.Play("Ghost_Scared");
                break;
            case GhostState.Recovering:
                animator.Play("Ghost_ScaredRecovering");
                break;
            case GhostState.Dead:
                animator.Play("Ghost_Dead");
                break;
        }
    }

    public void Pause(bool pause)
    {
        isPaused = pause;
    }

    public void ResetGhost()
    {
        transform.position = startPosition;
        ChooseRandomDirection();
    }

}
