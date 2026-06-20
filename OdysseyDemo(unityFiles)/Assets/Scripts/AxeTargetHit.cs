using System.Collections;
using UnityEngine;

public class AxeTargetHit : MonoBehaviour
{
    [Header("Camera")]
    public CameraPOVToggle cameraPOVToggle;

    [Header("Door Animator")]
    public Animator doorAnimator;
    public string doorOpenTriggerName = "DoorOpened";

    [Header("Audio")]
    public SFXAudio sfxAudio;

    private bool hasBeenHit = false;

    private void Start()
    {
        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenHit)
            return;

        if (!other.CompareTag("Arrow"))
            return;

        hasBeenHit = true;

        if (sfxAudio != null)
            sfxAudio.PlayPuzzleComplete();

        StartCoroutine(HitSequence());
    }

    private IEnumerator HitSequence()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].enabled = false;
        }

        yield return new WaitForSeconds(1f);

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
            cameraPOVToggle.enabled = false;
        }

        if (doorAnimator != null)
            doorAnimator.SetTrigger(doorOpenTriggerName);

        Destroy(gameObject);
    }
}