using UnityEngine;
using TMPro;

public class ShipTransportManager : MonoBehaviour
{
    [Header("PLAYER")]
    public Transform player;
    public Rigidbody playerRigidbody;
    public TPS playerTPS;
    public CameraPOVToggle cameraPOVToggle;

    [Header("SHIP")]
    public Transform shipParent;
    public ShipMovement shipMovement;
    public Rigidbody shipRigidbody;

    [Header("PLAYER POSITIONS")]
    public Transform playerPositionOnShip;
    public Transform playerExitPosition;

    [Header("ROTATION OFFSETS")]
    public float boardingYRotationOffset = -90f;
    public float exitYRotationOffset = 0f;

    [Header("SHIP RESPAWN AFTER PLAYER DEATH")]
    public bool resetShipPositionWhenPlayerDies = true;
    public Transform shipResetPosition;

    [Header("BOARDING UI BEFORE CONTROL")]
    public GameObject shipControlIntroUI;
    public TMP_Text shipControlIntroText;
    public string shipControlMessage = "Use the up and down arrow keys to move the ship. To turn the ship around use the left and right arrows keys.";
    public KeyCode closeIntroUIKey = KeyCode.Return;

    [Header("SETTINGS")]
    public bool forceThirdPersonWhenBoarding = true;
    public bool disableShipMovementUntilBoarded = true;

    [Header("RUNTIME INFO")]
    public bool isPlayerOnShip = false;
    public bool waitingForShipControlConfirm = false;

    private bool originalRigidbodyKinematic;
    private Vector3 originalShipPosition;
    private Quaternion originalShipRotation;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            if (playerRigidbody == null)
            {
                playerRigidbody = player.GetComponent<Rigidbody>();
            }

            if (playerTPS == null)
            {
                playerTPS = player.GetComponent<TPS>();
            }
        }

        if (cameraPOVToggle == null && Camera.main != null)
        {
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();
        }

        if (shipRigidbody == null && shipParent != null)
        {
            shipRigidbody = shipParent.GetComponent<Rigidbody>();
        }

        if (playerRigidbody != null)
        {
            originalRigidbodyKinematic = playerRigidbody.isKinematic;
        }

        if (shipParent != null)
        {
            originalShipPosition = shipParent.position;
            originalShipRotation = shipParent.rotation;
        }

        if (disableShipMovementUntilBoarded && shipMovement != null)
        {
            shipMovement.enabled = false;
        }

        if (shipControlIntroUI != null)
        {
            shipControlIntroUI.SetActive(false);
        }
    }

    void Update()
    {
        if (!waitingForShipControlConfirm)
        {
            return;
        }

        if (Input.GetKeyDown(closeIntroUIKey))
        {
            CloseShipIntroUIAndEnableControls();
        }
    }

    public void BoardShip()
    {
        if (isPlayerOnShip)
        {
            return;
        }

        if (player == null || shipParent == null || playerPositionOnShip == null)
        {
            return;
        }

        isPlayerOnShip = true;
        waitingForShipControlConfirm = true;

        if (forceThirdPersonWhenBoarding && cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
        }

        if (playerTPS != null)
        {
            playerTPS.ForceStandAfterRespawn();
            playerTPS.enabled = false;
        }

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        player.SetParent(shipParent, true);

        player.position = playerPositionOnShip.position;

        player.rotation =
            playerPositionOnShip.rotation *
            Quaternion.Euler(0f, boardingYRotationOffset, 0f);

        if (shipMovement != null)
        {
            shipMovement.enabled = false;
        }

        OpenShipIntroUI();
    }

    void OpenShipIntroUI()
    {
        if (shipControlIntroText != null)
        {
            shipControlIntroText.text = shipControlMessage;
        }

        if (shipControlIntroUI != null)
        {
            shipControlIntroUI.SetActive(true);
        }
    }

    void CloseShipIntroUIAndEnableControls()
    {
        waitingForShipControlConfirm = false;

        if (shipControlIntroUI != null)
        {
            shipControlIntroUI.SetActive(false);
        }

        if (isPlayerOnShip && shipMovement != null)
        {
            shipMovement.enabled = true;
        }
    }

    public void ExitShip()
    {
        if (!isPlayerOnShip)
        {
            return;
        }

        if (player == null || playerExitPosition == null)
        {
            return;
        }

        isPlayerOnShip = false;
        waitingForShipControlConfirm = false;

        if (shipControlIntroUI != null)
        {
            shipControlIntroUI.SetActive(false);
        }

        StopShipControls();

        player.SetParent(null, true);

        player.position = playerExitPosition.position;

        player.rotation =
            playerExitPosition.rotation *
            Quaternion.Euler(0f, exitYRotationOffset, 0f);

        RestorePlayerAfterShip();
    }

    public void ForceLeaveShipBecausePlayerDied()
    {
        if (!isPlayerOnShip)
        {
            return;
        }

        isPlayerOnShip = false;
        waitingForShipControlConfirm = false;

        if (shipControlIntroUI != null)
        {
            shipControlIntroUI.SetActive(false);
        }

        StopShipControls();

        if (player != null)
        {
            player.SetParent(null, true);
        }

        if (resetShipPositionWhenPlayerDies)
        {
            ResetShipPosition();
        }

        RestorePlayerAfterShip();
    }

    void StopShipControls()
    {
        if (shipMovement != null)
        {
            shipMovement.enabled = false;
        }

        if (shipRigidbody != null)
        {
            shipRigidbody.linearVelocity = Vector3.zero;
            shipRigidbody.angularVelocity = Vector3.zero;
        }
    }

    void RestorePlayerAfterShip()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = originalRigidbodyKinematic;
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.enabled = true;
            cameraPOVToggle.ForceThirdPerson();
            cameraPOVToggle.enabled = false;
        }

        if (playerTPS != null)
        {
            playerTPS.enabled = true;
            playerTPS.ForceStandAfterRespawn();
        }
    }

    void ResetShipPosition()
    {
        if (shipParent == null)
        {
            return;
        }

        if (shipResetPosition != null)
        {
            shipParent.position = shipResetPosition.position;
            shipParent.rotation = shipResetPosition.rotation;
        }
        else
        {
            shipParent.position = originalShipPosition;
            shipParent.rotation = originalShipRotation;
        }

        if (shipRigidbody != null)
        {
            shipRigidbody.linearVelocity = Vector3.zero;
            shipRigidbody.angularVelocity = Vector3.zero;
        }
    }
}