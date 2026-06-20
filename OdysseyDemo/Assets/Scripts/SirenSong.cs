using UnityEngine;

public class SirenSong : MonoBehaviour
{
[Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Settings")]
    public bool stopAudioWhenLeaving = true;

    private void Start()
    {
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (audioSource != null && stopAudioWhenLeaving)
        {
            audioSource.Stop();
        }
    }
}
