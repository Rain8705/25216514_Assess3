using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    [Header("Settings")]
    public GameObject cherryPrefab;
    public float respawnDelay = 5f;
    public Vector2 minBounds = new Vector2(-14f, -13f);
    public Vector2 maxBounds = new Vector2(11f, 14f);
    public LayerMask wallLayer;
    public float checkRadius = 0.35f;

    bool isActive = true;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        if (cherryPrefab)
        {
            Vector2 startPos = GetRandomValidPosition();
            if (startPos != Vector2.zero)
            {
                cherryPrefab.transform.position = startPos;
                cherryPrefab.SetActive(true);
            }
            else
            {
                cherryPrefab.SetActive(false);
                Debug.LogWarning("CherryController: No valid starting position found!");
            }
        }
    }

    public void HideCherryTemporarily()
    {
        if (!cherryPrefab || !isActive) return;
        cherryPrefab.SetActive(false);
        isActive = false;
        StartCoroutine(RespawnAfterDelay());
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (!cherryPrefab) yield break;

        Vector2 newPos = GetRandomValidPosition();
        if (newPos != Vector2.zero)
        {
            cherryPrefab.transform.position = newPos;
            cherryPrefab.SetActive(true);
            isActive = true;
        }
    }

    Vector2 GetRandomValidPosition()
    {
        const int MAX_TRIES = 200;
        int tries = 0;

        while (tries < MAX_TRIES)
        {
            float randX = Random.Range(minBounds.x, maxBounds.x);
            float randY = Random.Range(minBounds.y, maxBounds.y);
            Vector2 p = new Vector2(Mathf.Round(randX * 2f) / 2f, Mathf.Round(randY * 2f) / 2f);

            bool hitWall = Physics2D.OverlapCircle(p, checkRadius, wallLayer);
            bool inRestricted = IsInRestrictedZone(p);

            if (!hitWall && !inRestricted)
                return p;

            tries++;
        }

        Debug.LogWarning("CherryController: No valid random spawn found after max attempts!");
        return Vector2.zero;
    }

    bool IsInRestrictedZone(Vector2 pos)
    {
        return
            IsInside(pos, -14f, -2f, -11f, -4f) ||
            IsInside(pos, -14f, 5f, -11f, 3f) ||
            IsInside(pos, 8f, 5f, 11f, 3f) ||
            IsInside(pos, 8f, -2f, 11f, -4f) ||
            IsInside(pos, -4f, 2f, 1f, -1f);
    }

    bool IsInside(Vector2 p, float xMin, float yMin, float xMax, float yMax)
    {
        float margin = 0.25f;
        float left = Mathf.Min(xMin, xMax) - margin;
        float right = Mathf.Max(xMin, xMax) + margin;
        float bottom = Mathf.Min(yMin, yMax) - margin;
        float top = Mathf.Max(yMin, yMax) + margin;

        return (p.x >= left && p.x <= right && p.y >= bottom && p.y <= top);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        DrawZone(-14, -2, -11, -4);
        DrawZone(-14, 5, -11, 3);
        DrawZone(8, 5, 11, 3);
        DrawZone(8, -2, 11, -4);
        DrawZone(-4, 2, 1, -1);
    }

    void DrawZone(float xMin, float yMin, float xMax, float yMax)
    {
        Vector3 center = new Vector3((xMin + xMax) / 2f, (yMin + yMax) / 2f, 0);
        Vector3 size = new Vector3(Mathf.Abs(xMax - xMin), Mathf.Abs(yMax - yMin), 0.1f);
        Gizmos.DrawCube(center, size);
    }
#endif
}
