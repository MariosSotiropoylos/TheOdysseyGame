using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("REFERENCES")]
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("SHOOTING SETTINGS")]
    public float fireRate = 1.75f;
    public float detectionRange = 10f;

    [Header("LOOK AT PLAYER")]
    public bool alwaysLookAtPlayer = true;
    public float rotationSpeed = 8f;

    [Header("VISIBLE DETECTION AREA")]
    public Transform detectionAreaVisual;
    public float detectionAreaYOffset = -1.2f;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    private float nextFireTime;

    void Start()
    {
        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();

        if (detectionAreaVisual != null)
        {
            detectionAreaVisual.gameObject.SetActive(true);
            UpdateDetectionAreaVisual();
        }
    }

    void Update()
    {
        if (player == null || firePoint == null || projectilePrefab == null)
            return;

        if (alwaysLookAtPlayer)
            LookAtPlayer();

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && Time.time >= nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + fireRate;
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void ShootAtPlayer()
    {
        if (sfxAudio != null)
            sfxAudio.PlaySirenShoot();

        Vector3 direction = (player.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();

        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
        }
        else
        {
            Destroy(projectile, 5f);
        }
    }

    void UpdateDetectionAreaVisual()
    {
        if (detectionAreaVisual == null)
            return;

        detectionAreaVisual.localPosition = new Vector3(
            0f,
            detectionAreaYOffset,
            0f
        );

        detectionAreaVisual.localScale = new Vector3(
            detectionRange * 2f,
            0.02f,
            detectionRange * 2f
        );
    }
}