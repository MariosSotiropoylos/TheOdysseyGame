using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitlePage : MonoBehaviour
{
    [Header("UI Image")]
    public Image titleImage;

    [Header("Sound")]
    public AudioSource enterSound;

    [Header("Fade Settings")]
    public float fadeToBlackDuration = 2f;
    public float waitAfterBlack = 1f;

    [Header("Scene")]
    public string sceneToLoad = "SampleScene";

    [Header("Runtime State")]
    public bool transitionStarted = false;

    private void Start()
    {
        if (titleImage == null)
        {
            titleImage = GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (transitionStarted)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            transitionStarted = true;

            if (enterSound != null)
            {
                enterSound.Play();
            }

            StartCoroutine(FadeToBlackAndLoadScene());
        }
    }

    private IEnumerator FadeToBlackAndLoadScene()
    {
        if (titleImage == null)
        {
            yield break;
        }

        Color startColor = titleImage.color;
        Color targetColor = Color.black;

        float elapsedTime = 0f;

        while (elapsedTime < fadeToBlackDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / fadeToBlackDuration;

            titleImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        titleImage.color = Color.black;

        yield return new WaitForSeconds(waitAfterBlack);

        SceneManager.LoadScene(sceneToLoad);
    }
}