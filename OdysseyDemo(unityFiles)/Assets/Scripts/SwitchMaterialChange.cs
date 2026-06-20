using UnityEngine;

public class SwitchMaterialChange : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Input")]
    public KeyCode interactKey = KeyCode.I;

    [Header("Interaction UI")]
    public GameObject interactionUI;

    [Header("Renderer")]
    public Renderer objectRenderer;

    [Header("Materials")]
    public Material material1;
    public Material material2;
    public Material material3;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("Runtime Info")]
    public bool playerInside = false;
    public int currentMaterialIndex = -1;

    private void Start()
    {
        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();

        if (objectRenderer == null)
            objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
            objectRenderer = GetComponentInChildren<Renderer>();

        HideInteractionUI();
    }

    private void Update()
    {
        if (!playerInside)
            return;

        ShowInteractionUI();

        if (Input.GetKeyDown(interactKey))
            ChangeToNextMaterial();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = true;
        ShowInteractionUI();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = false;
        HideInteractionUI();
    }

    void ChangeToNextMaterial()
    {
        if (sfxAudio != null)
            sfxAudio.PlaySwitch();

        currentMaterialIndex++;

        if (currentMaterialIndex > 2)
            currentMaterialIndex = 0;

        ApplyMaterial();
    }

    void ApplyMaterial()
    {
        if (objectRenderer == null)
            return;

        if (currentMaterialIndex == 0 && material1 != null)
            objectRenderer.sharedMaterial = material1;
        else if (currentMaterialIndex == 1 && material2 != null)
            objectRenderer.sharedMaterial = material2;
        else if (currentMaterialIndex == 2 && material3 != null)
            objectRenderer.sharedMaterial = material3;
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
}