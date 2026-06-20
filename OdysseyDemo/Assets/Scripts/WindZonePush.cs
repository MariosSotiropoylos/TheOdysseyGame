using UnityEngine;

public class WindZonePush : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Wind Direction")]
    public Vector3 windDirection = Vector3.left;

    [Header("Wind Strength")]
    public float pushForce = 600f;

    [Header("Wind Timing")]
    public bool windStartsActive = true;
    public float activeDuration = 3f;
    public float inactiveDuration = 2f;

    [Header("Wind Particles")]
    public ParticleSystemRenderer[] windParticleRenderers;

    [Header("Runtime Info")]
    public bool playerInside = false;
    public bool windActive = true;

    private Rigidbody playerRigidbody;
    private float timer = 0f;

    private void Start()
    {
        windActive = windStartsActive;

        if (windActive)
        {
            timer = activeDuration;
        }
        else
        {
            timer = inactiveDuration;
        }


        if (windParticleRenderers == null || windParticleRenderers.Length == 0)
        {
            windParticleRenderers = GetComponentsInChildren<ParticleSystemRenderer>(true);
        }

        SetParticlesVisible(windActive);
    }

    private void Update()
    {
        HandleWindTimer();
    }

    private void HandleWindTimer()
    {
        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            return;
        }

        windActive = !windActive;

        if (windActive)
        {
            timer = activeDuration;
            SetParticlesVisible(true);
        }
        else
        {
            timer = inactiveDuration;
            SetParticlesVisible(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = true;
        playerRigidbody = other.GetComponent<Rigidbody>();

        if (playerRigidbody == null)
        {
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = false;
        playerRigidbody = null;
    }

    private void FixedUpdate()
    {
        if (!windActive)
        {
            return;
        }

        if (!playerInside || playerRigidbody == null)
        {
            return;
        }

        Vector3 direction = windDirection.normalized;

        playerRigidbody.AddForce(direction * pushForce, ForceMode.Acceleration);
    }

    private void SetParticlesVisible(bool visible)
    {
        if (windParticleRenderers == null)
        {
            return;
        }

        for (int i = 0; i < windParticleRenderers.Length; i++)
        {
            if (windParticleRenderers[i] != null)
            {
                windParticleRenderers[i].enabled = visible;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = windActive ? Color.cyan : Color.gray;

        Vector3 start = transform.position;
        Vector3 end = start + windDirection.normalized * 2f;

        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.15f);
    }
}