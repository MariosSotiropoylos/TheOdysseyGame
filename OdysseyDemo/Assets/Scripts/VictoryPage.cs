using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryPage : MonoBehaviour
{
    [Header("UI Image")]
    public Image VictoryImage;

    [Header("Fade Settings")]
    public float fadeToBlackDuration = 2f;
    public float waitAfterBlack = 1f;

    [Header("Scene")]
    public string sceneToLoad = "TitleScene";

    [Header("Runtime State")]
    public bool transitionStarted = false;

    private void Start()
    {
        if (VictoryImage == null)
        {
            VictoryImage = GetComponent<Image>();
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
            StartCoroutine(FadeToBlackAndLoadScene());
        }
    }

    private IEnumerator FadeToBlackAndLoadScene()
    {
        if (VictoryImage == null)
        {
            yield break;
        }

        Color startColor = VictoryImage.color;
        Color targetColor = Color.black;

        float elapsedTime = 0f;

        while (elapsedTime < fadeToBlackDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / fadeToBlackDuration;

            VictoryImage.color = Color.Lerp(
                startColor,
                targetColor,
                t
            );

            yield return null;
        }

        VictoryImage.color = Color.black;

        yield return new WaitForSeconds(waitAfterBlack);

        SceneManager.LoadScene(sceneToLoad);
    }
}