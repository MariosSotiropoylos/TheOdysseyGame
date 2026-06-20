using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    [Header("Slider")]
    public Slider sfxSlider; // Max value should be 100!!!!!!

    [Header("SFX Audio Sources")]
    public AudioSource[] sfxAudioSources;

    [Header("Original Volumes")]
    public float[] baseVolumes;

    private void Start()
    {
        if (sfxSlider == null)
        {
            sfxSlider = GetComponent<Slider>();
        }

        if (sfxAudioSources != null)
        {
            baseVolumes = new float[sfxAudioSources.Length];

            for (int i = 0; i < sfxAudioSources.Length; i++)
            {
                if (sfxAudioSources[i] != null)
                {
                    baseVolumes[i] = sfxAudioSources[i].volume;
                }
            }
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);
            UpdateSFXVolume(sfxSlider.value);
        }
    }

    public void UpdateSFXVolume(float sliderValue)
    {
        float volumeMultiplier = sliderValue / 100f;

        for (int i = 0; i < sfxAudioSources.Length; i++)
        {
            if (sfxAudioSources[i] != null)
            {
                sfxAudioSources[i].volume = baseVolumes[i] * volumeMultiplier;
            }
        }
    }
}