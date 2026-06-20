using UnityEngine;

public class NympthDialogueTrigger : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Player Reference")]
    public Transform playerTransform;
    public TPS playerTPS;
    public Animator playerAnimator;
    public string idleStateName = "Idle";

    [Header("Input")]
    public KeyCode interactKey = KeyCode.I;

    [Header("Dialogue Script To Activate")]
    public NympthDialogue nympthDialogue;

    [Header("Interaction UI")]
    public GameObject interactionUI;

    [Header("Camera POV")]
    public CameraPOVToggle cameraPOVToggle;
    public bool forceThirdPersonBeforeDialogue = true;

    [Header("Face Interaction Settings")]
    public bool turnPlayerTowardThisObject = true;

    private bool playerInside = false;

    private void Start()
    {
        if (nympthDialogue != null)
        {
            nympthDialogue.enabled = false;
        }

        if (cameraPOVToggle == null && Camera.main != null)
        {
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();
        }

        HideInteractionUI();
    }

    private void Update()
    {
        if (!playerInside)
        {
            return;
        }

        if (IsDialogueActive())
        {
            HideInteractionUI();
            return;
        }

        ShowInteractionUI();

        if (Input.GetKeyDown(interactKey))
        {
            ActivateDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = true;

        if (playerTransform == null)
        {
            playerTransform = other.transform;
        }

        if (playerTPS == null)
        {
            playerTPS = other.GetComponent<TPS>();
        }

        if (playerAnimator == null)
        {
            playerAnimator = other.GetComponent<Animator>();
        }

        if (!IsDialogueActive())
        {
            ShowInteractionUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = false;
        HideInteractionUI();
    }

    private void ActivateDialogue()
    {
        if (nympthDialogue == null)
        {
            return;
        }

        if (forceThirdPersonBeforeDialogue && cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
        }

        if (turnPlayerTowardThisObject)
        {
            TurnPlayerTowardInteraction();
        }

        ForcePlayerIdleAnimation();

        HideInteractionUI();

        nympthDialogue.enabled = false;
        nympthDialogue.enabled = true;
    }

    private void ForcePlayerIdleAnimation()
    {
        if (playerTPS != null)
        {
            playerTPS.ForceStandAfterRespawn();
            return;
        }

        if (playerAnimator == null)
        {
            return;
        }

        playerAnimator.ResetTrigger("AttackTrigger");
        playerAnimator.ResetTrigger("JumpTrigger");
        playerAnimator.ResetTrigger("DeathTrigger");

        playerAnimator.SetBool("IsWalking", false);
        playerAnimator.SetBool("IsRunning", false);
        playerAnimator.SetBool("IsCrouching", false);
        playerAnimator.SetBool("IsSneaking", false);
        playerAnimator.SetBool("IsGrounded", true);
        playerAnimator.SetBool("IsDead", false);

        playerAnimator.Play(idleStateName, 0, 0f);
    }

    private void TurnPlayerTowardInteraction()
    {
        if (playerTransform == null)
        {
            return;
        }

        Vector3 direction = transform.position - playerTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        playerTransform.rotation = targetRotation;
    }

    private bool IsDialogueActive()
    {
        return nympthDialogue != null && nympthDialogue.enabled;
    }

    private void ShowInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }
    }

    private void HideInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}