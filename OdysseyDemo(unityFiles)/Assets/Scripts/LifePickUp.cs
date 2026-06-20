using UnityEngine;

public class LifePickup : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";

    [Header("LIFE REWARD")]
    public int livesToGive = 1;

    [Header("PICKUPT SETTINGS")]
    public bool destroyAfterPickup = true;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        LivesManager livesManager = other.GetComponent<LivesManager>();

        if (livesManager == null)
        {
            livesManager = other.GetComponentInParent<LivesManager>();
        }

        if (livesManager == null)
        {
            livesManager = FindFirstObjectByType<LivesManager>();
        }

        if (livesManager == null)
        {
            return;
        }

        collected = true;

        livesManager.AddLife(livesToGive);
		
        if (destroyAfterPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}