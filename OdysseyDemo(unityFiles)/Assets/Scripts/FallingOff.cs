using UnityEngine;

public class FallingOff : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
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

        triggered = true;

        healthManager.FallIntoBottomlessPit();

        Invoke(nameof(ResetTrigger), 0.5f);
    }

    void ResetTrigger()
    {
        triggered = false;
    }
}