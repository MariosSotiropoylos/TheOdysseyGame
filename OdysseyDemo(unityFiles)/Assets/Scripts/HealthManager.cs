using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("Death / Respawn Settings")]
    public float lethalHitDelayBeforeDeath = 0.18f;
    public float deathAnimationDuration = 2.98f;

    [Header("References")]
    public LivesManager livesManager;
    public CheckpointManager checkpointManager;
    public CameraPOVToggle cameraPOVToggle;
    public TPS playerController;
    public Animator playerAnimator;
    public string idleStateName = "Idle";

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("Health UI")]
    public Image[] filledHeartImages;

    [Header("Cyclops Encounter")]
    public CyclopsTime cyclopsTime;

    [Header("Suitor Encounter")]
    public SuitorHealth[] suitorsToReset;

    [Header("Ship Transport")]
    public ShipTransportManager shipTransportManager;

    [Header("Runtime Info")]
    public int currentHealth;
    public bool isDead = false;
    public bool isInvincible = false;

    private bool isDying = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (playerController == null)
        {
            playerController = GetComponent<TPS>();
        }

        if (checkpointManager == null)
        {
            checkpointManager = FindFirstObjectByType<CheckpointManager>();
        }

        if (cameraPOVToggle == null && Camera.main != null)
        {
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();
        }

        if (shipTransportManager == null)
        {
            shipTransportManager = FindFirstObjectByType<ShipTransportManager>();
        }

        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }

        if (sfxAudio == null)
        {
            sfxAudio = FindFirstObjectByType<SFXAudio>();
        }

        UpdateHealthUI();

        Debug.Log("Player health started: " + currentHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (!CanTakeDamage())
        {
            return;
        }

        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            if (sfxAudio != null)
            {
                sfxAudio.PlayDeath();
            }

            StartCoroutine(HandleLethalHitDelay());
        }
        else
        {
            if (sfxAudio != null)
            {
                sfxAudio.PlayDamage();
            }
        }
    }

    public bool CanTakeDamage()
    {
        if (isDead)
        {
            return false;
        }

        if (isInvincible)
        {
            return false;
        }

        if (isDying)
        {
            return false;
        }

        return true;
    }

    IEnumerator HandleLethalHitDelay()
    {
        isDying = true;
        isInvincible = true;
        currentHealth = 0;

        UpdateHealthUI();

        yield return new WaitForSeconds(lethalHitDelayBeforeDeath);

        StartCoroutine(HandlePlayerDeath());
    }

    IEnumerator HandlePlayerDeath()
    {
        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
        }

        if (cyclopsTime != null)
        {
            cyclopsTime.ResetCountdownAfterPlayerDeath();
        }

        if (shipTransportManager != null && shipTransportManager.isPlayerOnShip)
        {
            shipTransportManager.ForceLeaveShipBecausePlayerDied();
        }

        isDead = true;

        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.IsDead = true;
        }

        bool hasLivesLeft = true;

        if (livesManager != null)
        {
            hasLivesLeft = livesManager.LoseLife();
        }

        yield return new WaitForSeconds(deathAnimationDuration);

        if (hasLivesLeft)
        {
            RespawnHealthOnly();
        }
    }

    void RespawnHealthOnly()
    {
        currentHealth = maxHealth;
        isDead = false;
        isInvincible = false;
        isDying = false;

        if (checkpointManager != null)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = false;
            }

            transform.position = checkpointManager.GetRespawnPosition();
            transform.rotation = checkpointManager.GetRespawnRotation();
        }

        transform.SetParent(null, true);

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.enabled = true;
            cameraPOVToggle.ForceThirdPerson();
            cameraPOVToggle.enabled = false;
        }

        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.IsDead = false;
            playerController.ForceStandAfterRespawn();
        }

        ResetSuitorsAfterPlayerDeath();

        ForceRespawnState();

        UpdateHealthUI();
    }

    void ResetSuitorsAfterPlayerDeath()
    {
        if (suitorsToReset == null)
        {
            return;
        }

        for (int i = 0; i < suitorsToReset.Length; i++)
        {
            if (suitorsToReset[i] != null)
            {
                suitorsToReset[i].ResetSuitorAfterPlayerDeath();
            }
        }
    }

    void UpdateHealthUI()
    {
        if (filledHeartImages == null || filledHeartImages.Length == 0)
        {
            return;
        }

        for (int i = 0; i < filledHeartImages.Length; i++)
        {
            if (filledHeartImages[i] != null)
            {
                filledHeartImages[i].enabled = i < currentHealth;
            }
        }
    }

    public bool Heal(int healAmount)
    {
        if (isDead || isDying)
        {
            return false;
        }

        if (currentHealth >= maxHealth)
        {
            return false;
        }

        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();

        if (sfxAudio != null)
        {
            sfxAudio.PlayHealth();
        }
		
        return true;
    }

    public void FallIntoBottomlessPit()
    {
        if (isDying || isDead)
        {
            return;
        }

        StartCoroutine(HandleBottomlessPitFall());
    }

    IEnumerator HandleBottomlessPitFall()
    {
        isDying = true;
        isInvincible = true;

        if (sfxAudio != null)
        {
            sfxAudio.PlayDeath();
        }

        if (cyclopsTime != null)
        {
            cyclopsTime.ResetCountdownAfterPlayerDeath();
        }

        if (shipTransportManager != null && shipTransportManager.isPlayerOnShip)
        {
            shipTransportManager.ForceLeaveShipBecausePlayerDied();
        }

        bool hasLivesLeft = true;

        if (livesManager != null)
        {
            hasLivesLeft = livesManager.LoseLife();
        }

        if (hasLivesLeft)
        {
            RespawnHealthOnly();
        }

        yield return null;
    }

    void ForceRespawnState()
    {
        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
        }

        if (playerAnimator == null)
        {
            return;
        }

        playerAnimator.ResetTrigger("DeathTrigger");
        playerAnimator.ResetTrigger("JumpTrigger");
        playerAnimator.ResetTrigger("AttackTrigger");

        playerAnimator.SetBool("IsDead", false);
        playerAnimator.SetBool("IsWalking", false);
        playerAnimator.SetBool("IsRunning", false);
        playerAnimator.SetBool("IsCrouching", false);
        playerAnimator.SetBool("IsSneaking", false);
        playerAnimator.SetBool("IsGrounded", true);

        playerAnimator.Play(idleStateName, 0, 0f);
    }
}