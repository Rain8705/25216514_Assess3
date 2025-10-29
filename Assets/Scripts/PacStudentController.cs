using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;  // tiles per second
    public LayerMask wallLayer;

    [Header("Particles & Effects")]
    public ParticleSystem wallBumpParticles;
    public ParticleSystem deathParticles;

    Rigidbody2D rb;
    Animator anim;
    AudioManager audioMgr;
    AudioSource moveAudio;
    SpriteRenderer sr; // <— for flipping

    Vector2 targetPos;
    Vector2 currentInput = Vector2.right;
    Vector2 lastInput = Vector2.zero;
    bool isMoving = false;

    Vector3 spawnPos;
    Coroutine moveRoutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        audioMgr = FindFirstObjectByType<AudioManager>();

        moveAudio = gameObject.AddComponent<AudioSource>();
        moveAudio.loop = true;
        moveAudio.playOnAwake = false;
        moveAudio.volume = 0.7f;

        spawnPos = transform.position;
        targetPos = transform.position;

        if (rb)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    void Update()
    {
        if (GameManager.I == null || !GameManager.I.canPlayerMove)
        {
            StopMoveAudio();
            if (anim) anim.SetBool("isMoving", false);
            return;
        }

        // --- Handle input ---
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector2.right;

        if (!isMoving) TryMove();
    }

    void TryMove()
    {
        if (CanMove(lastInput))
        {
            currentInput = lastInput;
            StartMove(currentInput);
        }
        else if (CanMove(currentInput))
        {
            StartMove(currentInput);
        }
        else
        {
            StopMoveAudio();
            if (wallBumpParticles) wallBumpParticles.Play();
            if (audioMgr) audioMgr.PlaySFX(audioMgr.sfxCollideWall);
            if (anim) anim.SetBool("isMoving", false);
        }
    }

    void StartMove(Vector2 dir)
    {
        if (isMoving) return;

        targetPos = (Vector2)transform.position + dir;
        isMoving = true;

        // --- Animator parameters ---
        if (anim)
        {
            anim.SetBool("isMoving", true);
            anim.SetFloat("MoveX", dir.x);
            anim.SetFloat("MoveY", dir.y);
        }

        // --- Flip sprite when moving left/right ---
        if (sr != null)
        {
            if (dir.x < 0) sr.flipX = true;
            else if (dir.x > 0) sr.flipX = false;
        }

        // --- Play move audio ---
        StartMoveAudio();

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(LerpMove(transform.position, targetPos));
    }

    IEnumerator LerpMove(Vector2 start, Vector2 end)
    {
        float elapsed = 0f;
        float duration = 1f / moveSpeed;

        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isMoving = false;

        StopMoveAudio();
        if (anim) anim.SetBool("isMoving", false);
    }

    bool CanMove(Vector2 dir)
    {
        if (dir == Vector2.zero) return false;
        Vector2 check = (Vector2)transform.position + dir * 0.5f;
        return !Physics2D.OverlapCircle(check, 0.28f, wallLayer);
    }

    // -------------------- AUDIO HELPERS --------------------
    void StartMoveAudio()
    {
        if (audioMgr && audioMgr.sfxMove && moveAudio)
        {
            if (moveAudio.clip != audioMgr.sfxMove)
                moveAudio.clip = audioMgr.sfxMove;

            if (!moveAudio.isPlaying)
                moveAudio.Play();
        }
    }

    void StopMoveAudio()
    {
        if (moveAudio && moveAudio.isPlaying)
            moveAudio.Stop();
    }

    // -------------------- COLLISION HANDLING --------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            Destroy(other.gameObject);
            GameManager.I.hud.AddScore(10);
            GameManager.I.pelletsRemaining--;
            if (GameManager.I.pelletsRemaining <= 0)
                GameManager.I.OnAllPelletsEaten();

            if (audioMgr) audioMgr.PlaySFX(audioMgr.sfxEatPellet);
        }
        else if (other.CompareTag("PowerPill"))
        {
            Destroy(other.gameObject);
            GameManager.I.hud.AddScore(50);
            GameManager.I.StartScaredMode();
            if (audioMgr) audioMgr.PlaySFX(audioMgr.sfxEatPellet);
        }
        else if (other.CompareTag("Bonus"))
        {
            GameManager.I.hud.AddScore(100);
            if (audioMgr) audioMgr.PlaySFX(audioMgr.sfxEatPellet);
            CherryController cc = FindFirstObjectByType<CherryController>();
            if (cc) cc.HideCherryTemporarily();
        }
        else if (other.CompareTag("Ghost"))
        {
            GameManager.I.hud.LoseLife();
            GameManager.I.OnPacDied();
        }
    }

    // -------------------- STATE CONTROL --------------------
    public void AwaitInput()
    {
        isMoving = false;
        StopMoveAudio();
        if (anim) anim.SetBool("isMoving", false);
    }

    public void Respawn()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        transform.position = spawnPos;
        targetPos = spawnPos;
        currentInput = Vector2.right;
        lastInput = Vector2.zero;
        isMoving = false;
        StopMoveAudio();
        if (anim) anim.SetBool("isMoving", false);
    }

    public void PlayDeathParticles()
    {
        if (deathParticles) deathParticles.Play();
        StopMoveAudio();
    }
    public void TeleportTo(Vector2 pos, Vector2 continueDir)
    {
        transform.position = pos;
        targetPos = pos + continueDir;
        currentInput = continueDir;
        lastInput = continueDir;
        isMoving = false;

        if (anim) anim.SetBool("isMoving", true);
    }

}
