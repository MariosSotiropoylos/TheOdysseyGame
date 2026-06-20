using System.Collections;
using UnityEngine;
using TMPro;

public class CyclopsTime : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text countdownText;

    [Header("COUNTDOWN")]
    public float countdownStartValue = 15f;

    [Header("CYCLOPS REFERENCES")]
    public Animator cyclopsAnimator;
    public CyclopsBehaviour cyclopsBehaviour;
    public Collider cyclopsCollider;

    [Header("ANIMATOR")]
    public string timeTriggerName = "TimeTrigger";
    public string huntingBoolName = "IsHunting";

    [Header("OBJECT MOVE AFTER TIMER")]
    public Transform objectToMove;
    public Transform moveTarget;
    public float delayBeforeMove = 5f;
    public float moveDuration = 3f;

    [Header("ENTRANCE OBJECT")]
    public GameObject EntranceRock;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("FORCE PLAYER TO LOOK")]
    public bool forcePlayerToLookWhileObjectMoves = true;
    public Transform playerTransform;
    public TPS playerTPS;
    public CameraPOVToggle cameraPOVToggle;
    public Transform cameraTransform;
    public Transform originalCameraPosition;
    public float lookRotationSpeed = 8f;

    [Header("RUNTIME STATE")]
    public bool countdownRunning = false;
    public bool countdownFinished = false;
    public bool objectMoveStarted = false;
    public bool objectMoveFinished = false;
    public bool playerIsForcedToLook = false;

    private Coroutine countdownRoutine;
    private Coroutine moveRoutine;
    private int lastDisplayedSecond = -1;

    void Start()
    {
        HideCountdownText();

        if (sfxAudio == null)
        {
            sfxAudio = FindFirstObjectByType<SFXAudio>();
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraPOVToggle == null && Camera.main != null)
        {
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();
        }

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }

        if (playerTPS == null && playerTransform != null)
        {
            playerTPS = playerTransform.GetComponent<TPS>();
        }
    }

    public void StartCountdown()
    {
        if (countdownRunning || countdownFinished)
        {
            return;
        }

        countdownRoutine = StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        if (EntranceRock != null)
        {
            EntranceRock.SetActive(true);
        }

        countdownRunning = true;
        lastDisplayedSecond = -1;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        float currentTime = countdownStartValue;

        while (currentTime > 0f)
        {
            int displayedSecond = Mathf.CeilToInt(currentTime);

            if (countdownText != null)
            {
                countdownText.text = displayedSecond.ToString();
            }

            if (displayedSecond != lastDisplayedSecond)
            {
                lastDisplayedSecond = displayedSecond;

                if (sfxAudio != null)
                {
                    sfxAudio.PlayClock();
                }
            }

            currentTime -= Time.deltaTime;
            yield return null;
        }

        FinishCountdown();
    }

    void FinishCountdown()
    {
        countdownRunning = false;
        countdownFinished = true;

        HideCountdownText();

        if (cyclopsBehaviour != null)
        {
            cyclopsBehaviour.PlayCyclopsDeathOnce();
            cyclopsBehaviour.enabled = false;
        }

        if (cyclopsCollider != null)
        {
            cyclopsCollider.enabled = false;
        }

        if (cyclopsAnimator != null)
        {
            cyclopsAnimator.SetBool(huntingBoolName, false);
            cyclopsAnimator.SetTrigger(timeTriggerName);
        }

        if (EntranceRock != null)
        {
            EntranceRock.SetActive(false);
        }

        if (moveRoutine == null)
        {
            moveRoutine = StartCoroutine(MoveObjectAfterDelay());
        }
    }

    IEnumerator MoveObjectAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeMove);

        if (objectToMove == null || moveTarget == null)
        {
            yield break;
        }

        objectMoveStarted = true;

        if (sfxAudio != null)
        {
            sfxAudio.PlayBoulderMoving();
        }

        if (forcePlayerToLookWhileObjectMoves)
        {
            BeginForcePlayerLook();
        }

        Vector3 startPosition = objectToMove.position;
        Vector3 targetPosition = moveTarget.position;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            objectToMove.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (forcePlayerToLookWhileObjectMoves)
            {
                ForceLookAtObject();
            }

            yield return null;
        }

        objectToMove.position = targetPosition;

        if (forcePlayerToLookWhileObjectMoves)
        {
            ForceLookAtObject();
            EndForcePlayerLook();
        }

        objectMoveFinished = true;
    }

    void BeginForcePlayerLook()
    {
        playerIsForcedToLook = true;

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
            cameraPOVToggle.enabled = false;
        }

        if (playerTPS != null)
        {
            playerTPS.enabled = false;
        }
    }

    void ForceLookAtObject()
    {
        if (objectToMove == null)
        {
            return;
        }

        if (playerTransform != null)
        {
            Vector3 playerDirection = objectToMove.position - playerTransform.position;
            playerDirection.y = 0f;

            if (playerDirection.sqrMagnitude > 0.001f)
            {
                Quaternion playerTargetRotation = Quaternion.LookRotation(playerDirection);

                playerTransform.rotation = Quaternion.Slerp(
                    playerTransform.rotation,
                    playerTargetRotation,
                    lookRotationSpeed * Time.deltaTime
                );
            }
        }

        if (cameraTransform != null)
        {
            Vector3 cameraDirection = objectToMove.position - cameraTransform.position;

            if (cameraDirection.sqrMagnitude > 0.001f)
            {
                Quaternion cameraTargetRotation = Quaternion.LookRotation(cameraDirection);

                cameraTransform.rotation = Quaternion.Slerp(
                    cameraTransform.rotation,
                    cameraTargetRotation,
                    lookRotationSpeed * Time.deltaTime
                );
            }
        }
    }

    void EndForcePlayerLook()
    {
        playerIsForcedToLook = false;

        ReturnCameraToOriginalPosition();

        if (playerTPS != null)
        {
            playerTPS.enabled = true;
        }
    }

    void ReturnCameraToOriginalPosition()
    {
        if (cameraTransform == null || originalCameraPosition == null)
        {
            return;
        }

        cameraTransform.position = originalCameraPosition.position;
        cameraTransform.rotation = originalCameraPosition.rotation;
    }

    public void ResetCountdownAfterPlayerDeath()
    {
        if (countdownFinished)
        {
            return;
        }

        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }

        countdownRunning = false;
        lastDisplayedSecond = -1;

        if (EntranceRock != null)
        {
            EntranceRock.SetActive(false);
        }

        HideCountdownText();

        if (cyclopsBehaviour != null)
        {
            cyclopsBehaviour.ResetForNewAttempt();
        }

        if (cyclopsCollider != null)
        {
            cyclopsCollider.enabled = true;
        }

        if (cyclopsAnimator != null)
        {
            cyclopsAnimator.SetBool(huntingBoolName, false);
        }
    }

    void HideCountdownText()
    {
        if (countdownText != null)
        {
            countdownText.text = "";
            countdownText.gameObject.SetActive(false);
        }
    }
}