using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";

    [Header("HEAL SETTINGS")]
    public int healAmount = 1;

    [Header("PICKUP SETTINGS")]
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

        HealthManager healthManager = other.GetComponent<HealthManager>();

        if (healthManager == null)
        {
            healthManager = other.GetComponentInParent<HealthManager>();
        }

        if (healthManager == null)
        {
            return;
        }

        bool healed = healthManager.Heal(healAmount);

        if (!healed)
        {
            return;
        }

        collected = true;

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