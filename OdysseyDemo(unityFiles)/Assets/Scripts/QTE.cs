using System.Collections;
using UnityEngine;
using TMPro;

public class QTE : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Input To Start QTE")]
    public KeyCode interactKey = KeyCode.I;

    [Header("QTE Inputs")]
    public KeyCode firstQTEKey = KeyCode.A;
    public KeyCode secondQTEKey = KeyCode.W;
    public KeyCode thirdQTEKey = KeyCode.D;

    [Header("QTE Settings")]
    public float timeLimitPerInput = 3f;
    public bool canRepeatAfterSuccess = false;

    [Header("UI")]
    public GameObject interactionUI;
    public GameObject qtePanel;

    [Header("QTE Images")]
    public GameObject firstQTEImage;
    public GameObject secondQTEImage;
    public GameObject thirdQTEImage;

    [Header("Countdown Text")]
    public TMP_Text qteCountdownText;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("Camera Positions")]
    public Transform cameraTransform;
    public Transform originalCameraPosition;
    public Transform firstQTECameraPosition;
    public Transform secondQTECameraPosition;
    public Transform thirdQTECameraPosition;

    [Header("Area Visual")]
    public MeshRenderer areaMeshRenderer;

    [Header("Object To Fade And Destroy On Success")]
    public GameObject objectToFadeAndDestroy;
    public Renderer objectRendererToFade;
    public float fadeDuration = 2f;

    [Header("Player Scripts")]
    public TPS playerTPS;
    public CameraPOVToggle cameraPOVToggle;

    [Header("Runtime State")]
    public bool playerInside = false;
    public bool qteActive = false;
    public bool qteCompleted = false;
    public bool mustExitBeforeRetry = false;
    public int currentQTEIndex = 0;
    public bool fadeStarted = false;

    private KeyCode[] qteKeys;
    private GameObject[] qteImages;
    private Transform[] qteCameraPositions;

    private float currentTimer = 0f;
    private int lastDisplayedSecond = -1;

    void Start()
    {
        qteKeys = new KeyCode[] { firstQTEKey, secondQTEKey, thirdQTEKey };
        qteImages = new GameObject[] { firstQTEImage, secondQTEImage, thirdQTEImage };
        qteCameraPositions = new Transform[] { firstQTECameraPosition, secondQTECameraPosition, thirdQTECameraPosition };

        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraPOVToggle == null && Camera.main != null)
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();

        if (areaMeshRenderer == null)
            areaMeshRenderer = GetComponent<MeshRenderer>();

        FindFadeRenderer();

        HideInteractionUI();
        HideQTEUI();
    }

    void Update()
    {
        if (!playerInside)
            return;

        if (qteCompleted && !canRepeatAfterSuccess)
        {
            HideInteractionUI();
            return;
        }

        if (mustExitBeforeRetry)
        {
            HideInteractionUI();
            HideQTEUI();
            return;
        }

        if (!qteActive)
        {
            ShowInteractionUI();

            if (Input.GetKeyDown(interactKey))
                StartQTE();

            return;
        }

        HandleQTE();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = true;

        if (playerTPS == null)
            playerTPS = other.GetComponent<TPS>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraPOVToggle == null && Camera.main != null)
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();

        if (!qteActive && !mustExitBeforeRetry && (!qteCompleted || canRepeatAfterSuccess))
            ShowInteractionUI();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = false;
        mustExitBeforeRetry = false;

        HideInteractionUI();
        HideQTEUI();
    }

    void StartQTE()
    {
        qteActive = true;
        currentQTEIndex = 0;

        HideInteractionUI();
        ShowQTEUI();

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
            cameraPOVToggle.enabled = false;
        }

        if (playerTPS != null)
            playerTPS.enabled = false;

        StartCurrentQTEInput();
    }

    void StartCurrentQTEInput()
    {
        currentTimer = timeLimitPerInput;
        lastDisplayedSecond = -1;

        MoveCameraToCurrentQTEPosition();

        if (qteCountdownText != null)
            qteCountdownText.gameObject.SetActive(true);

        UpdateQTEUI();
    }

    void HandleQTE()
    {
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0f)
        {
            FailQTE();
            return;
        }

        KeyCode neededKey = qteKeys[currentQTEIndex];

        if (Input.GetKeyDown(neededKey))
        {
            SucceedCurrentInput();
            return;
        }

        if (AnyQTEKeyPressed())
        {
            FailQTE();
            return;
        }

        UpdateQTEUI();
    }

    bool AnyQTEKeyPressed()
    {
        for (int i = 0; i < qteKeys.Length; i++)
        {
            if (Input.GetKeyDown(qteKeys[i]))
                return true;
        }

        return false;
    }

    void SucceedCurrentInput()
    {
        currentQTEIndex++;

        if (currentQTEIndex >= qteKeys.Length)
        {
            CompleteQTE();
            return;
        }

        StartCurrentQTEInput();
    }

    void FailQTE()
    {
        qteActive = false;
        currentQTEIndex = 0;
        mustExitBeforeRetry = true;
        lastDisplayedSecond = -1;

        HideQTEUI();
        HideInteractionUI();

        ReturnCameraToOriginalPosition();
        ReactivatePlayerControl();
    }

    void CompleteQTE()
    {
        qteActive = false;
        qteCompleted = true;
        mustExitBeforeRetry = false;
        lastDisplayedSecond = -1;

        if (sfxAudio != null)
            sfxAudio.PlayPuzzleComplete();

        HideQTEUI();
        HideInteractionUI();

        ReturnCameraToOriginalPosition();
        ReactivatePlayerControl();

        if (areaMeshRenderer != null)
            areaMeshRenderer.enabled = false;

        StartFadeAndDestroyObject();
    }

    void ReactivatePlayerControl()
    {
        if (playerTPS != null)
            playerTPS.enabled = true;

        if (cameraPOVToggle != null)
            cameraPOVToggle.enabled = true;
    }

    void MoveCameraToCurrentQTEPosition()
    {
        if (cameraTransform == null || qteCameraPositions == null)
            return;

        if (currentQTEIndex < 0 || currentQTEIndex >= qteCameraPositions.Length)
            return;

        Transform targetCameraPosition = qteCameraPositions[currentQTEIndex];

        if (targetCameraPosition == null)
            return;

        cameraTransform.position = targetCameraPosition.position;
        cameraTransform.rotation = targetCameraPosition.rotation;
    }

    void ReturnCameraToOriginalPosition()
    {
        if (cameraTransform == null || originalCameraPosition == null)
            return;

        cameraTransform.position = originalCameraPosition.position;
        cameraTransform.rotation = originalCameraPosition.rotation;
    }

    void UpdateQTEUI()
    {
        ShowOnlyCurrentQTEImage();

        int displayedSecond = Mathf.CeilToInt(currentTimer);

        if (qteCountdownText != null)
            qteCountdownText.text = displayedSecond.ToString();

        if (displayedSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = displayedSecond;

            if (sfxAudio != null)
                sfxAudio.PlayClock();
        }
    }

    void ShowOnlyCurrentQTEImage()
    {
        if (qteImages == null)
            return;

        for (int i = 0; i < qteImages.Length; i++)
        {
            if (qteImages[i] != null && qteImages[i].activeSelf != (i == currentQTEIndex))
                qteImages[i].SetActive(i == currentQTEIndex);
        }
    }

    void HideAllQTEImages()
    {
        if (qteImages == null)
            return;

        for (int i = 0; i < qteImages.Length; i++)
        {
            if (qteImages[i] != null && qteImages[i].activeSelf)
                qteImages[i].SetActive(false);
        }
    }

    void FindFadeRenderer()
    {
        if (objectRendererToFade != null || objectToFadeAndDestroy == null)
            return;

        objectRendererToFade = objectToFadeAndDestroy.GetComponent<Renderer>();

        if (objectRendererToFade == null)
            objectRendererToFade = objectToFadeAndDestroy.GetComponentInChildren<Renderer>();
    }

    void StartFadeAndDestroyObject()
    {
        if (fadeStarted)
            return;

        if (objectToFadeAndDestroy == null)
            return;

        FindFadeRenderer();

        if (objectRendererToFade == null)
        {
            Destroy(objectToFadeAndDestroy);
            return;
        }

        fadeStarted = true;
        StartCoroutine(FadeObjectAndDestroy());
    }

    IEnumerator FadeObjectAndDestroy()
    {
        Material fadeMaterial = objectRendererToFade.material;

        Color startColor = fadeMaterial.color;
        float startAlpha = startColor.a;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / fadeDuration;
            Color newColor = fadeMaterial.color;
            newColor.a = Mathf.Lerp(startAlpha, 0f, t);
            fadeMaterial.color = newColor;

            yield return null;
        }

        Color finalColor = fadeMaterial.color;
        finalColor.a = 0f;
        fadeMaterial.color = finalColor;

        Destroy(objectToFadeAndDestroy);
        Destroy(fadeMaterial);
    }

    void ShowInteractionUI()
    {
        if (interactionUI != null && !interactionUI.activeSelf)
            interactionUI.SetActive(true);
    }

    void HideInteractionUI()
    {
        if (interactionUI != null && interactionUI.activeSelf)
            interactionUI.SetActive(false);
    }

    void ShowQTEUI()
    {
        if (qtePanel != null && !qtePanel.activeSelf)
            qtePanel.SetActive(true);

        if (qteCountdownText != null && !qteCountdownText.gameObject.activeSelf)
            qteCountdownText.gameObject.SetActive(true);
    }

    void HideQTEUI()
    {
        if (qtePanel != null && qtePanel.activeSelf)
            qtePanel.SetActive(false);

        if (qteCountdownText != null && qteCountdownText.gameObject.activeSelf)
            qteCountdownText.gameObject.SetActive(false);

        HideAllQTEImages();
    }
}