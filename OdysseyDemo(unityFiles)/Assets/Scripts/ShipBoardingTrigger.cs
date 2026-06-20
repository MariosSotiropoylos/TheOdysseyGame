using UnityEngine;

public class ShipBoardingTrigger : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";

    [Header("INPUT")]
    public KeyCode interactKey = KeyCode.I;

    [Header("INTERACTION UI")]
    public GameObject interactionUI;

    [Header("MANAGER")]
    public ShipTransportManager shipTransportManager;

    [Header("RUNTIME INFO")]
    public bool playerInside = false;

    private void Start()
    {
        if (shipTransportManager == null)
        {
            shipTransportManager = FindFirstObjectByType<ShipTransportManager>();
        }

        HideInteractionUI();
    }

    private void Update()
    {
        if (!playerInside)
        {
            return;
        }

        if (shipTransportManager == null)
        {
            HideInteractionUI();
            return;
        }

        if (shipTransportManager.isPlayerOnShip)
        {
            HideInteractionUI();
            return;
        }

        ShowInteractionUI();

        if (Input.GetKeyDown(interactKey))
        {
            BoardShip();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = true;

        if (shipTransportManager != null && !shipTransportManager.isPlayerOnShip)
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

    void BoardShip()
    {
        if (shipTransportManager == null)
        {
            return;
        }

        if (shipTransportManager.isPlayerOnShip)
        {
            HideInteractionUI();
            return;
        }

        HideInteractionUI();

        shipTransportManager.BoardShip();
    }

    void ShowInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }
    }

    void HideInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}