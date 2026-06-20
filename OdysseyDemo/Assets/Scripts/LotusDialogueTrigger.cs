using UnityEngine;

public class LotusDialogueTrigger : MonoBehaviour
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
    public LotusEaterDialogue lotusEaterDialogue;

    [Header("Interaction UI")]
    public GameObject interactionUI;

    [Header("Camera POV")]
    public CameraPOVToggle cameraPOVToggle;
    public bool forceThirdPersonBeforeDialogue = true;

    [Header("Settings")]
    public bool activateOnlyOnce = false;

    [Header("Face Interaction Settings")]
    public bool turnPlayerTowardThisObject = true;

    private bool activated = false;
    private bool playerInside = false;

    // After pressing I, the UI stays hidden until the player leaves.
    private bool mustExitBeforeShowingAgain = false;

    private void Start()
    {
        if (lotusEaterDialogue != null)
        {
            lotusEaterDialogue.enabled = false;
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

        if (activated && activateOnlyOnce)
        {
            HideInteractionUI();
            return;
        }

        if (IsDialogueActive() || mustExitBeforeShowingAgain)
        {
            HideInteractionUI();
            return;
        }

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

        if (activated && activateOnlyOnce)
        {
            HideInteractionUI();
            return;
        }

        if (IsDialogueActive() || mustExitBeforeShowingAgain)
        {
            HideInteractionUI();
            return;
        }

        ShowInteractionUI();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = false;
        HideInteractionUI();

        if (!IsDialogueActive())
        {
            mustExitBeforeShowingAgain = false;
        }
    }

    private void ActivateDialogue()
    {
        if (lotusEaterDialogue == null)
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

        activated = true;

        mustExitBeforeShowingAgain = true;
        HideInteractionUI();

        lotusEaterDialogue.enabled = false;
        lotusEaterDialogue.enabled = true;
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

        // Keep rotation only horizontal.
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
        return lotusEaterDialogue != null && lotusEaterDialogue.enabled;
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