using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SuitorHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 3f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;

    [Header("Damage Flash Settings")]
    public Material damageFlashMaterial;
    public float flashDuration = 0.12f;
    public int flashCount = 1;

    [Header("Death Settings")]
    public bool disableCollidersOnDeath = true;
    public bool disableRigidbodyOnDeath = true;
    public bool disableNavMeshAgentOnDeath = true;
    public bool disableEnemyAIOnDeath = true;

    [Header("Object To Show On Death")]
    public GameObject objectToShowOnDeath;

    [Header("Animator")]
    public Animator enemyAnimator;
    public string walkingBoolName = "IsWalking";
    public string deathTriggerName = "DeathTrigger";
    public string attackTriggerName = "AttackTrigger";
    public string idleStateName = "Idle";

    [Header("Scripts To Disable On Death")]
    public SuitorBehaviour SuitorBehaviour;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("Reset Settings")]
    public bool resetPositionWhenPlayerDies = true;

    private float currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    private bool isFlashing = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Renderer[] cachedRenderers;
    private Material[][] cachedOriginalMaterials;

    void Start()
    {
        currentHealth = maxHealth;

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (enemyAnimator == null)
            enemyAnimator = GetComponent<Animator>();

        if (enemyAnimator == null)
            enemyAnimator = GetComponentInChildren<Animator>();

        if (SuitorBehaviour == null)
            SuitorBehaviour = GetComponent<SuitorBehaviour>();

        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();

        cachedRenderers = GetComponentsInChildren<Renderer>(true);
        cachedOriginalMaterials = new Material[cachedRenderers.Length][];

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            cachedOriginalMaterials[i] = cachedRenderers[i].sharedMaterials;
        }

        if (objectToShowOnDeath != null)
            objectToShowOnDeath.SetActive(false);
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead)
            return;

        if (isInvincible)
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
        if (damageFlashMaterial == null)
            return;

        if (isFlashing)
            return;

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
                {
                    flashMaterials[j] = damageFlashMaterial;
                }

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
        if (cachedRenderers == null || cachedOriginalMaterials == null)
            return;

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null && i < cachedOriginalMaterials.Length)
            {
                cachedRenderers[i].sharedMaterials = cachedOriginalMaterials[i];
            }
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

        if (objectToShowOnDeath != null)
            objectToShowOnDeath.SetActive(true);

        if (disableEnemyAIOnDeath && SuitorBehaviour != null)
            SuitorBehaviour.enabled = false;

        if (disableNavMeshAgentOnDeath)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();

            if (agent != null && agent.enabled)
            {
                agent.isStopped = true;
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                agent.enabled = false;
            }
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool(walkingBoolName, false);
            enemyAnimator.ResetTrigger(attackTriggerName);
            enemyAnimator.SetTrigger(deathTriggerName);
        }

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

    public void ResetSuitorAfterPlayerDeath()
    {
        StopAllCoroutines();
        RestoreOriginalMaterials();
        CancelInvoke(nameof(EndInvincibility));

        currentHealth = maxHealth;
        isDead = false;
        isInvincible = false;
        isFlashing = false;

        if (objectToShowOnDeath != null)
            objectToShowOnDeath.SetActive(false);

        if (resetPositionWhenPlayerDies)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
                colliders[i].enabled = true;
        }

        if (SuitorBehaviour != null)
        {
            SuitorBehaviour.enabled = true;
            SuitorBehaviour.isDead = false;
            SuitorBehaviour.isAttacking = false;
            SuitorBehaviour.playerInArea = false;
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.ResetTrigger(deathTriggerName);
            enemyAnimator.ResetTrigger(attackTriggerName);
            enemyAnimator.SetBool(walkingBoolName, false);
            enemyAnimator.Play(idleStateName, 0, 0f);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        RestoreOriginalMaterials();
        CancelInvoke(nameof(EndInvincibility));
    }
}