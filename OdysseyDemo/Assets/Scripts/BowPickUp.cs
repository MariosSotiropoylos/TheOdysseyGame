using UnityEngine;
using TMPro;

public class BowPickUp : MonoBehaviour
{
    [Header("BOW")]
    public BowShooter bowShooter;

    [Header("POV SWITCH")]
    public CameraPOVToggle cameraPOVToggle;

    [Header("BOW REWARD IMAGE")]
    public GameObject BowImage;

    [Header("PICKUP UI BEFORE ACTIVATION")]
    public GameObject bowPickupUI;
    public TMP_Text bowPickupText;
    public string bowPickupMessage = "Press the F key to enter First Person Mode. Aim with the mouse and shoot arrows by pressing the right click.";
    public KeyCode closePickupUIKey = KeyCode.Return;

    [Header("PLAYER CONTROL")]
    public TPS playerTPS;
    public Rigidbody playerRigidbody;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("RUNTIME STATES")]
    public bool bowCollected = false;
    public bool waitingForPlayerConfirm = false;
    public bool cameraToggleActivated = false;
    public bool bowImageActivated = false;

    private Collider pickupCollider;
    private Renderer[] pickupRenderers;

    private void Start()
    {
        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();

        if (cameraPOVToggle == null && Camera.main != null)
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();

        if (BowImage != null)
            BowImage.SetActive(false);

        if (bowPickupUI != null)
            bowPickupUI.SetActive(false);

        pickupCollider = GetComponent<Collider>();
        pickupRenderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (!waitingForPlayerConfirm)
            return;

        if (Input.GetKeyDown(closePickupUIKey))
            CloseUIAndActivateBow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || bowCollected)
            return;

        bowCollected = true;
        waitingForPlayerConfirm = true;

        if (sfxAudio != null)
            sfxAudio.PlayCollectible();

        if (playerTPS == null)
            playerTPS = other.GetComponent<TPS>();

        if (playerRigidbody == null)
            playerRigidbody = other.GetComponent<Rigidbody>();

        FreezePlayer();
        HidePickupObjectVisualsAndCollider();
        OpenBowPickupUI();
    }

    void OpenBowPickupUI()
    {
        if (bowPickupText != null)
            bowPickupText.text = bowPickupMessage;

        if (bowPickupUI != null)
            bowPickupUI.SetActive(true);
    }

    void CloseUIAndActivateBow()
    {
        waitingForPlayerConfirm = false;

        if (bowPickupUI != null)
            bowPickupUI.SetActive(false);

        UnfreezePlayer();

        if (bowShooter != null)
            bowShooter.EnableBowShooting();

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.enabled = true;
            cameraToggleActivated = true;
        }
        else
        {
            cameraToggleActivated = false;
        }

        if (BowImage != null)
        {
            BowImage.SetActive(true);
            bowImageActivated = true;
        }
        else
        {
            bowImageActivated = false;
        }

        Destroy(gameObject);
    }

    void FreezePlayer()
    {
        if (playerTPS != null)
        {
            playerTPS.ForceStandAfterRespawn();
            playerTPS.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }
    }

    void UnfreezePlayer()
    {
        if (playerTPS != null)
        {
            playerTPS.enabled = true;
            playerTPS.ForceStandAfterRespawn();
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }
    }

    void HidePickupObjectVisualsAndCollider()
    {
        if (pickupCollider != null)
            pickupCollider.enabled = false;

        if (pickupRenderers == null)
            return;

        for (int i = 0; i < pickupRenderers.Length; i++)
        {
            if (pickupRenderers[i] != null)
                pickupRenderers[i].enabled = false;
        }
    }
}