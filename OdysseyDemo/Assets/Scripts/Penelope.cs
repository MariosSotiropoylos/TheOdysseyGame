using UnityEngine;
using UnityEngine.SceneManagement;

public class Penelope : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Scene")]
    public string victorySceneName = "VictoryScene";

    private bool sceneLoading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (sceneLoading)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        sceneLoading = true;

        SceneManager.LoadScene(victorySceneName);
    }
}