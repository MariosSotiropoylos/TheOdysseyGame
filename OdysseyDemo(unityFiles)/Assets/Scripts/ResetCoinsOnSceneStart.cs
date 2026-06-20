using UnityEngine;

public class ResetCoinsOnSceneStart : MonoBehaviour
{
    void Start()
    {
        CoinCollect.ResetCoins();
    }
}