using UnityEngine;
using UnityEngine.UI;

public class CollectToUnlockUI : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("UI To Unlock")]
    public Button buttonToActivate;
    public Image imageToShow;

    [Header("Script To Activate")]
    public ClickUIToOpen clickUIToOpenScript;

    [Header("Collect Settings")]
    public bool destroyAfterCollect = true;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    private bool collected = false;

    void Start()
    {
        if (sfxAudio == null)
        {
            sfxAudio = FindFirstObjectByType<SFXAudio>();
        }

        if (buttonToActivate != null)
        {
            buttonToActivate.interactable = false;
        }

        if (imageToShow != null)
        {
            imageToShow.gameObject.SetActive(false);
        }

        if (clickUIToOpenScript != null)
        {
            clickUIToOpenScript.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        collected = true;

        if (sfxAudio != null)
        {
            sfxAudio.PlayCollectible();
        }

        if (buttonToActivate != null)
        {
            buttonToActivate.interactable = true;
        }

        if (imageToShow != null)
        {
            imageToShow.gameObject.SetActive(true);
        }

        if (clickUIToOpenScript != null)
        {
            clickUIToOpenScript.enabled = true;
        }

        if (destroyAfterCollect)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}