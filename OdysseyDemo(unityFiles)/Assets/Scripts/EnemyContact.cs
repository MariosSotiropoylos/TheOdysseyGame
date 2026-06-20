using System.Collections;
using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Damage Settings")]
    public int contactDamage = 1;
    public float damageCooldown = 1.0f;

    [Header("Player Horizontal Knockback Settings")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.2f;

    [Header("Damage Flash Settings")]
    public Material damageFlashMaterial;
    public float flashDuration = 0.12f;
    public int flashCount = 3;

    private float lastDamageTime = -999f;
    private bool isFlashing = false;

    private Renderer[] cachedPlayerRenderers;
    private Material[][] cachedOriginalMaterials;

    private void OnCollisionEnter(Collision collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void TryDamagePlayer(GameObject otherObject)
    {
        if (!otherObject.CompareTag(playerTag))
            return;

        if (Time.time < lastDamageTime + damageCooldown)
            return;

        HealthManager playerHealth = otherObject.GetComponent<HealthManager>();

        if (playerHealth == null)
            return;

        if (!playerHealth.CanTakeDamage())
            return;

        lastDamageTime = Time.time;

        ApplyPlayerKnockback(otherObject);
        StartPlayerDamageFlash(otherObject);

        playerHealth.TakeDamage(contactDamage);
    }

    void ApplyPlayerKnockback(GameObject playerObject)
    {
        TPS playerController = playerObject.GetComponent<TPS>();

        if (playerController == null)
            return;

        Vector3 knockbackDirection = playerObject.transform.position - transform.position;
        knockbackDirection.y = 0f;

        if (knockbackDirection.sqrMagnitude < 0.001f)
            knockbackDirection = -transform.forward;

        knockbackDirection.Normalize();

        playerController.ApplyHorizontalKnockback(
            knockbackDirection,
            knockbackForce,
            knockbackDuration
        );
    }

    void StartPlayerDamageFlash(GameObject playerObject)
    {
        if (damageFlashMaterial == null)
            return;

        if (isFlashing)
            return;

        CachePlayerRenderers(playerObject);

        if (cachedPlayerRenderers == null || cachedPlayerRenderers.Length == 0)
            return;

        StartCoroutine(FlashPlayerMaterial());
    }

    void CachePlayerRenderers(GameObject playerObject)
    {
        cachedPlayerRenderers = playerObject.GetComponentsInChildren<Renderer>(true);
        cachedOriginalMaterials = new Material[cachedPlayerRenderers.Length][];

        for (int i = 0; i < cachedPlayerRenderers.Length; i++)
        {
            if (cachedPlayerRenderers[i] != null)
                cachedOriginalMaterials[i] = cachedPlayerRenderers[i].sharedMaterials;
        }
    }

    IEnumerator FlashPlayerMaterial()
    {
        isFlashing = true;

        for (int flash = 0; flash < flashCount; flash++)
        {
            for (int i = 0; i < cachedPlayerRenderers.Length; i++)
            {
                if (cachedPlayerRenderers[i] == null)
                    continue;

                Material[] flashMaterials = new Material[cachedPlayerRenderers[i].sharedMaterials.Length];

                for (int j = 0; j < flashMaterials.Length; j++)
                {
                    flashMaterials[j] = damageFlashMaterial;
                }

                cachedPlayerRenderers[i].sharedMaterials = flashMaterials;
            }

            yield return new WaitForSeconds(flashDuration);

            RestoreOriginalMaterials();

            yield return new WaitForSeconds(flashDuration);
        }

        isFlashing = false;
    }

    void RestoreOriginalMaterials()
    {
        if (cachedPlayerRenderers == null || cachedOriginalMaterials == null)
            return;

        for (int i = 0; i < cachedPlayerRenderers.Length; i++)
        {
            if (cachedPlayerRenderers[i] != null && i < cachedOriginalMaterials.Length)
                cachedPlayerRenderers[i].sharedMaterials = cachedOriginalMaterials[i];
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        RestoreOriginalMaterials();
        isFlashing = false;
    }
}