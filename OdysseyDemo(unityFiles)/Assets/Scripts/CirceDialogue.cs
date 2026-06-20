using UnityEngine;
using TMPro;

public class CirceDialogue : MonoBehaviour
{
    [Header("UI ELEMENTS")]
    public GameObject uiPanel;
    public TMP_Text dialogueText;

    [Header("ANSWER UI")]
    public GameObject answersPanel;
    public TMP_Text answerOneText;
    public TMP_Text answerTwoText;

    [Header("ANSWER TEXTS")]
    public string answerOne = "Kind Circe, I must return to my wife and son!";
    public string answerTwo = "Paradise? This is worse than hell you horrid hag.";

    [Header("CIRCE DIALOGUE TEXT")]
    public string firstLine = "Sweet Odysseus, do you truly want to leave? Forget Ithaca, let it become a mere fond memory.";
    public string questionText = "Why don't you stay with me in this paradise?";

    public string responseToAnswerOne =
        "Noble as always. I respect your honesty and your commitment to your family. May you have a safe passage back home, noble Odysseus.";

    public string responseToAnswerTwo =
        "Do you forget you address the great Circe? You shall pay for your hubris, you insolent mortal!";

    [Header("SELECTION VISUAL")]
    public string selectedPrefix = ">";
    public string normalPrefix = "";

    [Header("LIFE RESULT")]
    public LivesManager livesManager;
    public int livesGainedFromAnswerOne = 1;
    public int livesLostFromAnswerTwo = 1;
	
	[Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("DISABLE INTERACTION AFTER ANSWER")]
    public CirceDialogueTrigger circeDialogueTrigger;
    public Collider interactionCollider;
    public GameObject interactionUI;

    [Header("NPC ANIMATOR")]
    public Animator npcAnimator;
    public string talkingBoolName = "IsTalking";

    [Header("PLAYER & CAMERA SCRIPTS")]
    public GameObject player;
    public Rigidbody playerRigidbody;
    public TPS playerTPS;
    public CameraPOVToggle cameraPOVToggle;

    private int step = 0;
    private int selectedAnswer = 0;

    private bool choosingAnswer = false;
    private bool responseShown = false;
    private bool interactionDisabled = false;
    private bool lifeResultApplied = false;

    void OnEnable()
    {
        step = 0;
        selectedAnswer = 0;
        choosingAnswer = false;
        responseShown = false;
        interactionDisabled = false;
        lifeResultApplied = false;

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

        if (livesManager == null)
        {
            livesManager = FindFirstObjectByType<LivesManager>();
        }

        if (npcAnimator != null)
        {
            npcAnimator.SetBool(talkingBoolName, true);
        }

        if (cameraPOVToggle != null)
        {
            cameraPOVToggle.ForceThirdPerson();
            cameraPOVToggle.enabled = false;
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
		
		if (sfxAudio == null)
		{
			sfxAudio = FindFirstObjectByType<SFXAudio>();
		}

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }

        if (answersPanel != null)
        {
            answersPanel.SetActive(false);
        }

        if (dialogueText != null)
        {
            dialogueText.text = firstLine;
        }
    }

    void Update()
    {
        if (responseShown)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ApplyLifeResult();
                EndDialogue();
            }

            return;
        }

        if (!choosingAnswer)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                step++;

                if (step == 1)
                {
                    ShowQuestionAndAnswers();
                }
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            selectedAnswer = 0;
            UpdateAnswerVisuals();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            selectedAnswer = 1;
            UpdateAnswerVisuals();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChooseAnswer();
        }
    }

    void ShowQuestionAndAnswers()
    {
        choosingAnswer = true;
        selectedAnswer = 0;

        if (dialogueText != null)
        {
            dialogueText.text = questionText;
        }

        if (answersPanel != null)
        {
            answersPanel.SetActive(true);
        }

        UpdateAnswerVisuals();
    }

    void UpdateAnswerVisuals()
    {
        if (answerOneText != null)
        {
            answerOneText.text =
                (selectedAnswer == 0 ? selectedPrefix : normalPrefix) + answerOne;
        }

        if (answerTwoText != null)
        {
            answerTwoText.text =
                (selectedAnswer == 1 ? selectedPrefix : normalPrefix) + answerTwo;
        }
    }

    void ChooseAnswer()
    {
        choosingAnswer = false;
        responseShown = true;

        if (answersPanel != null)
        {
            answersPanel.SetActive(false);
        }

        if (selectedAnswer == 0)
        {
            if (dialogueText != null)
            {
                dialogueText.text = responseToAnswerOne;
            }
        }
        else
        {
            if (dialogueText != null)
            {
                dialogueText.text = responseToAnswerTwo;
            }
        }

        DisableFutureInteraction();
    }

	void ApplyLifeResult()
	{
		if (lifeResultApplied)
		{
			return;
		}

		lifeResultApplied = true;

		if (livesManager == null)
		{
			return;
		}

		if (selectedAnswer == 0)
		{
			livesManager.AddLife(livesGainedFromAnswerOne);
		}
		else
		{
			if (sfxAudio != null)
			{
				sfxAudio.PlayPig();
			}

			for (int i = 0; i < livesLostFromAnswerTwo; i++)
			{
				bool hasLivesLeft = livesManager.LoseLife();

				if (!hasLivesLeft)
				{
					break;
				}
			}
		}
	}

    void DisableFutureInteraction()
    {
        if (interactionDisabled)
        {
            return;
        }

        interactionDisabled = true;

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        if (circeDialogueTrigger != null)
        {
            circeDialogueTrigger.enabled = false;
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

        if (answersPanel != null)
        {
            answersPanel.SetActive(false);
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