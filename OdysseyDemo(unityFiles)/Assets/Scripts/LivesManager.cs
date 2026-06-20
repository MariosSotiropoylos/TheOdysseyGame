using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
    [Header("Lives Settings")]
    public int startingLives = 4;
    public int absoluteMaxLives = 99;

    [Header("Game Over Scene")]
    public string gameOverSceneName = "GameOverScene";

    [Header("UI")]
    public TMP_Text livesText;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("Runtime Info")]
    public int currentLives;

    void Start()
    {
        currentLives = startingLives;

        if (sfxAudio == null)
        {
            sfxAudio = FindFirstObjectByType<SFXAudio>();
        }

        UpdateLivesUI();

        Debug.Log("Lives started: " + currentLives);
    }

    public bool LoseLife()
    {
        currentLives--;

        if (currentLives < 0)
        {
            currentLives = 0;
        }

        UpdateLivesUI();

        if (currentLives <= 0)
        {
            GameOver();
            return false;
        }

        return true;
    }

    public void AddLife(int amount)
    {
        currentLives += amount;

        if (currentLives > absoluteMaxLives)
        {
            currentLives = absoluteMaxLives;
        }

        UpdateLivesUI();

        if (sfxAudio != null)
        {
            sfxAudio.PlayLife();
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "x " + currentLives;
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
}