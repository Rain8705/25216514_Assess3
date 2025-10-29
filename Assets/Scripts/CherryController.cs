using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;     // assign BonusItem in Inspector
    public float respawnDelay = 5f;     // time before reappearing
    public Vector2 minBounds = new Vector2(-8f, -8f);  // tweak to match map limits
    public Vector2 maxBounds = new Vector2(8f, 8f);

    private bool isActive = true;

    void Start()
    {
        if (cherryPrefab != null)
            cherryPrefab.SetActive(true);
    }

    public void HideCherryTemporarily()
    {
        if (cherryPrefab != null && isActive)
        {
            cherryPrefab.SetActive(false);
            isActive = false;
            StartCoroutine(RespawnAfterDelay());
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (cherryPrefab != null)
        {
            // Pick a random location within defined map bounds
            Vector2 newPos = new Vector2(
                Random.Range(minBounds.x, maxBounds.x),
                Random.Range(minBounds.y, maxBounds.y)
            );

            // Optionally: keep it on a grid if you want neat alignment
            newPos.x = Mathf.Round(newPos.x);
            newPos.y = Mathf.Round(newPos.y);

            cherryPrefab.transform.position = newPos;
            cherryPrefab.SetActive(true);
            isActive = true;
        }
    }
}
