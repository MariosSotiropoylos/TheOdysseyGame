using UnityEngine;
using TMPro;

public class CoinCollect : MonoBehaviour
{
    [Header("PLAYER DETECTION")]
    public string playerTag = "Player";

    [Header("COIN UI")]
    public TMP_Text coinText;

    [Header("COIN SETTINGS")]
    public int maxCoins = 999;

    [Header("LIFE REWARD")]
    public LivesManager livesManager;
    public int coinsNeededForLife = 10;
    public int livesToGive = 1;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    private static int currentCoins = 0;
    private static TMP_Text sharedCoinText;
    private static LivesManager sharedLivesManager;
    private static SFXAudio sharedSfxAudio;

    private bool collected = false;

    public static int CurrentCoins
    {
        get { return currentCoins; }
    }

    void Start()
    {
        if (coinText != null)
        {
            sharedCoinText = coinText;
        }

        if (livesManager != null)
        {
            sharedLivesManager = livesManager;
        }

        if (sfxAudio != null)
        {
            sharedSfxAudio = sfxAudio;
        }
        else if (sharedSfxAudio == null)
        {
            sharedSfxAudio = FindFirstObjectByType<SFXAudio>();
        }

        UpdateCoinUI();
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

        if (sharedSfxAudio != null)
        {
            sharedSfxAudio.PlayCoin();
        }

        if (currentCoins < maxCoins)
        {
            currentCoins++;
        }

        if (coinsNeededForLife > 0 && currentCoins % coinsNeededForLife == 0)
        {
            if (sharedLivesManager != null)
            {
                sharedLivesManager.AddLife(livesToGive);
            }
        }

        UpdateCoinUI();

        Destroy(gameObject);
    }

    public static bool HasCoins(int amount)
    {
        return currentCoins >= amount;
    }

    public static bool SpendCoins(int amount)
    {
        if (currentCoins < amount)
        {
            return false;
        }

        currentCoins -= amount;

        if (currentCoins < 0)
        {
            currentCoins = 0;
        }

        UpdateCoinUI();

        return true;
    }

    public static void ResetCoins()
    {
        currentCoins = 0;
        UpdateCoinUI();
    }

    static void UpdateCoinUI()
    {
        if (sharedCoinText != null)
        {
            sharedCoinText.text = "x " + currentCoins.ToString("000");
        }
    }
}