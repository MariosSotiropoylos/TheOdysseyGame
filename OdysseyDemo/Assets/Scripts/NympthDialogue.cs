using UnityEngine;
using TMPro;

public class NympthDialogue : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    public GameObject uiPanel;
    public TMP_Text dialogueText;

    [Header("DIALOGUE TEXT")]
    public string dialogueLine = "Odysseus? You always looked more like a Ulysses to me...";

    [Header("NPC ANIMATOR")]
    public Animator npcAnimator;
    public string talkingBoolName = "IsTalking";

    [Header("PLAYER & CAMERA SCRIPTS")]
    public GameObject player;
    public Rigidbody playerRigidbody;
    public TPS playerTPS;
    public CameraPOVToggle cameraPOVToggle;

    void OnEnable()
    {
        if (playerTPS == null && player != null)
        {
            playerTPS = player.GetComponent<TPS>();
        }

        if (playerRigidbody == null && player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
        }

        if (cameraPOVToggle == null && Camera.main != null)
        {
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();
        }

        if (npcAnimator != null)
        {
            npcAnimator.SetBool(talkingBoolName, true);
        }

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
            cameraPOVToggle.enabled = false;
        }

        if (playerTPS != null)
        {
            playerTPS.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }

        if (dialogueText != null)
        {
            dialogueText.text = dialogueLine;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        if (npcAnimator != null)
        {
            npcAnimator.SetBool(talkingBoolName, false);
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        if (playerTPS != null)
        {
            playerTPS.enabled = true;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
        }

        enabled = false;
    }
}