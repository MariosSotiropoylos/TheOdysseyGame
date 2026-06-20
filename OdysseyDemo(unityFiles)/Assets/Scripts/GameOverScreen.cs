using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Image")]
    public Image GameOverImage;

    [Header("Fade Settings")]
    public float fadeToBlackDuration = 3f;
    public float waitAfterBlack = 1f;

    [Header("Scene")]
    public string sceneToLoad = "SampleScene";

    [Header("Runtime State")]
    public bool transitionStarted = false;

    private void Start()
    {
        if (GameOverImage == null)
        {
            GameOverImage = GetComponent<Image>();
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
        if (GameOverImage == null)
        {
            yield break;
        }

        Color startColor = GameOverImage.color;
        Color targetColor = Color.black;

        float elapsedTime = 0f;

        while (elapsedTime < fadeToBlackDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / fadeToBlackDuration;

            GameOverImage.color = Color.Lerp(
                startColor,
                targetColor,
                t
            );

            yield return null;
        }

        GameOverImage.color = Color.black;

        yield return new WaitForSeconds(waitAfterBlack);

        SceneManager.LoadScene(sceneToLoad);
    }
}