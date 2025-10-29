using System.Collections;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public AudioManager audioManager;

    private Vector2 targetGridPos;
    private Vector2 direction = Vector2.zero;
    private Vector2 currentInput = Vector2.zero;
    private Vector2 lastInput = Vector2.zero;
    private bool isMoving = false;

    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioManager = FindFirstObjectByType<AudioManager>();

        // start facing right
        currentInput = Vector2.right;
        targetGridPos = transform.position;
    }

    void Update()
    {
        HandleInput();

        if (!isMoving)
        {
            TryMove();
        }
        else
        {
            MoveToTarget();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            lastInput = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S))
            lastInput = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A))
            lastInput = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D))
            lastInput = Vector2.right;
    }

    void TryMove()
    {
        // first check new input direction
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
            StopMove();
        }
    }

    void StartMove(Vector2 dir)
    {
        targetGridPos = (Vector2)transform.position + dir;
        isMoving = true;

        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }

        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.sfxMove);
        }
    }

    void StopMove()
    {
        isMoving = false;
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
    }

    void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetGridPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetGridPos) < 0.01f)
        {
            transform.position = targetGridPos;
            isMoving = false;
        }
    }

    bool CanMove(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, LayerMask.GetMask("Wall"));
        return hit.collider == null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // pellets
        if (collision.CompareTag("Pellet"))
        {
            audioManager.PlaySFX(audioManager.sfxEatPellet);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Bonus"))
        {
            Debug.Log("Cherry eaten!");
            audioManager.PlaySFX(audioManager.sfxEatPellet);

            CherryController cherryController = FindFirstObjectByType<CherryController>();
            if (cherryController != null)
            {
                cherryController.HideCherryTemporarily();
            }
        }

        // ghosts
        if (collision.CompareTag("Ghost"))
        {
            audioManager.PlaySFX(audioManager.sfxDeath);
            Debug.Log("PacStudent hit a ghost!");
            StopMove();
        }
    }
}
