using System.Collections;
using UnityEngine;

public class BowShooter : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public CameraPOVToggle cameraPOVToggle;
    public Animator bowAnimator;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public GameObject crosshair;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("Bow Unlock")]
    public GameObject bowVisual;
    public bool canShoot = false;

    [Header("Animation")]
    public string shootTriggerName = "ShootTrigger";
    public float arrowReleaseDelay = 0.8f;

    [Header("Arrow")]
    public float arrowSpeed = 45f;
    public float arrowLifeTime = 5f;

    [Header("Aiming")]
    public float maxAimDistance = 500f;

    private bool isShooting;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (cameraPOVToggle == null && playerCamera != null)
        {
            cameraPOVToggle = playerCamera.GetComponent<CameraPOVToggle>();
        }

        if (sfxAudio == null)
        {
            sfxAudio = FindFirstObjectByType<SFXAudio>();
        }

        UpdateBowVisibility();
    }

    private void Update()
    {
        UpdateBowVisibility();

        if (Input.GetMouseButtonDown(0) && CanUseBow() && !isShooting)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    private bool CanUseBow()
    {
        bool isFirstPerson = true;

        if (cameraPOVToggle != null)
        {
            isFirstPerson = cameraPOVToggle.isFirstPerson;
        }

        return canShoot &&
               isFirstPerson &&
               Cursor.lockState == CursorLockMode.Locked;
    }

    private void UpdateBowVisibility()
    {
        bool shouldShowBow = CanUseBow();

        if (bowVisual != null)
        {
            bowVisual.SetActive(shouldShowBow);
        }

        if (crosshair != null)
        {
            crosshair.SetActive(shouldShowBow);
        }
    }

    public void EnableBowShooting()
    {
        canShoot = true;
        UpdateBowVisibility();
    }

    public void DisableBowShooting()
    {
        canShoot = false;
        UpdateBowVisibility();
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;

        if (bowAnimator != null)
        {
            bowAnimator.SetTrigger(shootTriggerName);
        }

        yield return new WaitForSeconds(arrowReleaseDelay);

        FireArrow();

        if (sfxAudio != null)
        {
            sfxAudio.PlayArrow();
        }

        isShooting = false;
    }

    private void FireArrow()
    {
        if (playerCamera == null)
        {
            return;
        }

        if (arrowPrefab == null)
        {
            return;
        }

        if (arrowSpawnPoint == null)
        {
            return;
        }

        Vector3 shootDirection = GetAimDirection();

        GameObject arrow = Instantiate(
            arrowPrefab,
            arrowSpawnPoint.position,
            Quaternion.LookRotation(-shootDirection)
        );

        ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();

		if (projectile != null)
		{
			projectile.Launch(shootDirection, arrowSpeed, arrowLifeTime);
		}
		else
		{
			Destroy(arrow, arrowLifeTime);
		}
    }

    private Vector3 GetAimDirection()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance))
        {
            Vector3 directionToHit = hit.point - arrowSpawnPoint.position;
            return directionToHit.normalized;
        }

        Vector3 farPoint = playerCamera.transform.position + playerCamera.transform.forward * maxAimDistance;
        Vector3 directionToFarPoint = farPoint - arrowSpawnPoint.position;

        return directionToFarPoint.normalized;
    }
}