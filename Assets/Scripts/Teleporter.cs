using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleporter Settings")]
    public bool isLeftTeleporter;
    public Vector2 leftExit = new Vector2(-14f, 0.5f);
    public Vector2 rightExit = new Vector2(11f, 0.5f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PacStudentController pac = other.GetComponent<PacStudentController>();
        if (pac == null) return;

        Vector2 destination;

        if (isLeftTeleporter)
        {
            destination = rightExit;
            pac.TeleportTo(destination, Vector2.left);
        }
        else
        {
            destination = leftExit;
            pac.TeleportTo(destination, Vector2.right);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = isLeftTeleporter ? Color.cyan : Color.magenta;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
#endif
}
