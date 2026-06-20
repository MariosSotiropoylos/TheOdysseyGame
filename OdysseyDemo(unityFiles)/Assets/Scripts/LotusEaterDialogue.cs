using UnityEngine;
using TMPro;

public class LotusEaterDialogue : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    public GameObject uiPanel;
    public TMP_Text dialogueText;

    [Header("LOTUS REWARD IMAGE")]
    public GameObject lotusImage;

    [Header("COIN CHECK")]
    public int requiredCoins = 3;
    public bool spendCoinsOnPositiveAnswer = true;

    [Header("AFTER PURCHASE")]
    public bool itemBought = false;
    public LotusDialogueTrigger lotusDialogueTrigger;
    public Collider interactionCollider;

    [Header("OBJECT TO DELETE AFTER PURCHASE")]
    public GameObject objectToDeleteAfterPurchase;

    [Header("NPC ANIMATOR")]
    public Animator npcAnimator;
    public string talkingBoolName = "IsTalking";

    [Header("PLAYER & CAMERA SCRIPTS")]
    public GameObject player;
    public Rigidbody playerRigidbody;
    public TPS playerTPS;
    public CameraPOVToggle cameraPOVToggle;

    private int step = 0;
    private bool hasEnoughCoins = false;

    private string introText =
        "Hello stranger, I am afraid I can't let you pass without one of our delicious fruits. It only costs 3 coins, what a deal right?";

    private string negativeText =
        "It seems you don't quite have 3 coins... Oh well, I am sure that if you search around you will probably find more than enough.";

    private string positiveText =
        "Thank you kindly stranger. Huh, you don't want to eat it straight away? You are a weird one! But hey, to each his own...";

    void OnEnable()
    {
        if (itemBought)
        {
            enabled = false;
            return;
        }

        step = 0;

        if (playerTPS == null && player != null)
        {
            playerTPS = player.GetComponent<TPS>();
        }

        if (playerRigidbody == null && player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
        }

        if (cameraPOVToggle == null && Camera.main != null)
        {
            cameraPOVToggle = Camera.main.GetComponent<CameraPOVToggle>();
        }

        if (lotusImage != null)
        {
            lotusImage.SetActive(false);
        }

        if (npcAnimator != null)
        {
            npcAnimator.SetBool(talkingBoolName, true);
        }

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
        }

        if (playerTPS != null)
        {
            playerTPS.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }

        if (dialogueText != null)
        {
            dialogueText.text = introText;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            step++;

            if (step == 1)
            {
                CheckCoinsAndAnswer();
            }
            else if (step == 2)
            {
                EndDialogue();
            }
        }
    }

    void CheckCoinsAndAnswer()
    {
        hasEnoughCoins = CoinCollect.HasCoins(requiredCoins);

        if (hasEnoughCoins)
        {
            bool actuallySpentCoins = false;

            if (spendCoinsOnPositiveAnswer)
            {
                actuallySpentCoins = CoinCollect.SpendCoins(requiredCoins);
            }
            else
            {
                actuallySpentCoins = true;
            }

            if (dialogueText != null)
            {
                dialogueText.text = positiveText;
            }

            if (lotusImage != null)
            {
                lotusImage.SetActive(true);
            }

            if (actuallySpentCoins)
            {
                itemBought = true;

                DeleteObjectAfterPurchase();

                DisableFutureInteraction();
            }
        }
        else
        {
            if (dialogueText != null)
            {
                dialogueText.text = negativeText;
            }

            if (lotusImage != null)
            {
                lotusImage.SetActive(false);
            }
        }
    }

    void DeleteObjectAfterPurchase()
    {
        if (objectToDeleteAfterPurchase != null)
        {
            Destroy(objectToDeleteAfterPurchase);
        }
    }

    void DisableFutureInteraction()
    {
        if (lotusDialogueTrigger != null)
        {
            lotusDialogueTrigger.enabled = false;
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }
    }

    void EndDialogue()
    {
        if (npcAnimator != null)
        {
            npcAnimator.SetBool(talkingBoolName, false);
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        if (playerTPS != null)
        {
            playerTPS.enabled = true;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
        }

        enabled = false;
    }
}