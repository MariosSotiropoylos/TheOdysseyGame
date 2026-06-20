using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";

    [Header("CHECKPOINT MANAGER")]
    public CheckpointManager checkpointManager;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("SETTINGS")]
    public bool activateOnlyOnce = true;

    private bool activated = false;

    private GameObject inactiveFlag;
    private GameObject activatedFlag;

    void Start()
    {
        if (checkpointManager == null)
        {
            checkpointManager = FindFirstObjectByType<CheckpointManager>();
        }

        if (sfxAudio == null)
        {
            sfxAudio = FindFirstObjectByType<SFXAudio>();
        }

        if (transform.childCount >= 1)
        {
            inactiveFlag = transform.GetChild(0).gameObject;
        }

        if (transform.childCount >= 2)
        {
            activatedFlag = transform.GetChild(1).gameObject;
        }

        UpdateFlagVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated && activateOnlyOnce)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (checkpointManager == null)
        {
            return;
        }

        checkpointManager.SetCheckpoint(transform);

        if (sfxAudio != null)
        {
            sfxAudio.PlayCheckpoint();
        }

        activated = true;

        UpdateFlagVisuals();
    }

    private void UpdateFlagVisuals()
    {
        if (inactiveFlag != null)
        {
            inactiveFlag.SetActive(!activated);
        }

        if (activatedFlag != null)
        {
            activatedFlag.SetActive(activated);
        }
    }
}