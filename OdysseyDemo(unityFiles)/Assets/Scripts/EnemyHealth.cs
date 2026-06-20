using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 4f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;

    [Header("Damage Flash Settings")]
    public Material damageFlashMaterial;
    public float flashDuration = 0.12f;
    public int flashCount = 1;

    [Header("Death Settings")]
    public bool disableCollidersOnDeath = true;
    public bool disableRigidbodyOnDeath = true;

    [Header("Animator")]
    public Animator enemyAnimator;
    public string deathTriggerName = "DeathTrigger";

    [Header("Audio")]
    public SFXAudio sfxAudio;

    private float currentHealth;
    private bool isDead;
    private bool isInvincible;
    private bool isFlashing;

    private Renderer[] cachedRenderers;
    private Material[][] originalMaterials;

    void Start()
    {
        currentHealth = maxHealth;

        if (enemyAnimator == null)
            enemyAnimator = GetComponent<Animator>();

        if (enemyAnimator == null)
            enemyAnimator = GetComponentInChildren<Animator>();

        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();

        CacheRenderers();
    }

    void CacheRenderers()
    {
        cachedRenderers = GetComponentsInChildren<Renderer>(true);
        originalMaterials = new Material[cachedRenderers.Length][];

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null)
                originalMaterials[i] = cachedRenderers[i].sharedMaterials;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead || isInvincible)
            return;

        currentHealth -= damageAmount;

        if (currentHealth < 0f)
            currentHealth = 0f;

        if (sfxAudio != null)
            sfxAudio.PlayEnemyDamage();

        StartEnemyDamageFlash();

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        StartInvincibility();
    }

    void StartInvincibility()
    {
        isInvincible = true;
        CancelInvoke(nameof(EndInvincibility));
        Invoke(nameof(EndInvincibility), invincibilityDuration);
    }

    void EndInvincibility()
    {
        isInvincible = false;
    }

    void StartEnemyDamageFlash()
    {
        if (damageFlashMaterial == null || isFlashing)
            return;

        if (cachedRenderers == null || cachedRenderers.Length == 0)
            CacheRenderers();

        if (cachedRenderers == null || cachedRenderers.Length == 0)
            return;

        StartCoroutine(FlashEnemyMaterial());
    }

    IEnumerator FlashEnemyMaterial()
    {
        isFlashing = true;

        for (int flash = 0; flash < flashCount; flash++)
        {
            for (int i = 0; i < cachedRenderers.Length; i++)
            {
                if (cachedRenderers[i] == null)
                    continue;

                Material[] flashMaterials = new Material[cachedRenderers[i].sharedMaterials.Length];

                for (int j = 0; j < flashMaterials.Length; j++)
                    flashMaterials[j] = damageFlashMaterial;

                cachedRenderers[i].sharedMaterials = flashMaterials;
            }

            yield return new WaitForSeconds(flashDuration);

            RestoreOriginalMaterials();

            yield return new WaitForSeconds(flashDuration);
        }

        isFlashing = false;
    }

    void RestoreOriginalMaterials()
    {
        if (cachedRenderers == null || originalMaterials == null)
            return;

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null && i < originalMaterials.Length)
                cachedRenderers[i].sharedMaterials = originalMaterials[i];
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isInvincible = false;

        StopAllCoroutines();
        RestoreOriginalMaterials();
        CancelInvoke(nameof(EndInvincibility));

        if (enemyAnimator != null)
            enemyAnimator.SetTrigger(deathTriggerName);

        if (disableCollidersOnDeath)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                    colliders[i].enabled = false;
            }
        }

        if (disableRigidbodyOnDeath)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        RestoreOriginalMaterials();
        CancelInvoke(nameof(EndInvincibility));
        isFlashing = false;
    }
}