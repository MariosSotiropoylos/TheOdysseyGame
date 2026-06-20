using UnityEngine;

public class ShipExitTrigger : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";

    [Header("INPUT")]
    public KeyCode interactKey = KeyCode.I;

    [Header("INTERACTION UI")]
    public GameObject interactionUI;

    [Header("OBJECT TO DEACTIVATE ON EXIT")]
    public GameObject objectToDeactivateOnExit;

    [Header("MANAGER")]
    public ShipTransportManager shipTransportManager;

    [Header("RUNTIME INFO")]
    public bool playerInside = false;

    void Start()
    {
        if (shipTransportManager == null)
        {
            shipTransportManager = FindFirstObjectByType<ShipTransportManager>();
        }

        HideInteractionUI();
    }

    void Update()
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

        if (!shipTransportManager.isPlayerOnShip)
        {
            HideInteractionUI();
            return;
        }

        ShowInteractionUI();

        if (Input.GetKeyDown(interactKey))
        {
            ExitShip();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerInside = true;

        if (shipTransportManager != null && shipTransportManager.isPlayerOnShip)
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

    void ExitShip()
    {
        if (shipTransportManager == null)
        {
            return;
        }

        if (!shipTransportManager.isPlayerOnShip)
        {
            HideInteractionUI();
            return;
        }

        HideInteractionUI();

        shipTransportManager.ExitShip();

        if (objectToDeactivateOnExit != null)
        {
            objectToDeactivateOnExit.SetActive(false);
        }
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