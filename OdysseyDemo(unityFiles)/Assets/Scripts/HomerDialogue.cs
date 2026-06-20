using UnityEngine;
using TMPro;

public class HomerDialogue : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    public GameObject uiPanel;
    public TMP_Text dialogueText;

    [Header("PLAYER & CAMERA SCRIPTS")]
    public GameObject player;
    public Rigidbody playerRigidbody;
    public TPS playerTPS;
    public CameraPOVToggle cameraPOVToggle;

    private int step = 0;

    private string introText =
        "Greetings, O great and wise king of Ithaca. Let the Muses be kind and regale us with your weary tale of return.";

    private string controlsText =
        "Use the arrow keys to move and the mouse to control the camera. To jump press Space, to attack press X, and to crouch press C. If you want to pause, press ESC.";

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject;
        }

        if (playerTPS == null && player != null)
            playerTPS = player.GetComponent<TPS>();

        if (playerRigidbody == null && player != null)
            playerRigidbody = player.GetComponent<Rigidbody>();

        if (cameraPOVToggle == null && Camera.main != null)
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
            cameraPOVToggle.enabled = false;
        }

        if (playerTPS != null)
            playerTPS.enabled = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        Time.timeScale = 0f;

        if (uiPanel != null)
            uiPanel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = introText;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;

        step++;

        if (step == 1)
        {
            if (dialogueText != null)
                dialogueText.text = controlsText;
        }
        else if (step == 2)
        {
            if (uiPanel != null)
                uiPanel.SetActive(false);

            if (playerTPS != null)
                playerTPS.enabled = true;

            if (cameraPOVToggle != null)
                cameraPOVToggle.enabled = false;

            if (playerRigidbody != null)
            {
                playerRigidbody.isKinematic = false;
                playerRigidbody.linearVelocity = Vector3.zero;
                playerRigidbody.angularVelocity = Vector3.zero;
            }

            Time.timeScale = 1f;

            enabled = false;
        }
    }
}