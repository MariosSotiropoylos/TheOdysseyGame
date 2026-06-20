using UnityEngine;
using TMPro;

public class CurrentObjective : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";
	
    [Header("UI ELEMENTS")]
    public TMP_Text dialogueText;
	
	[Header("DIALOGUE TEXT")]
    public string dialogueLine = "Current Objective:";
	
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (dialogueText != null)
        {
            dialogueText.text = dialogueLine;
        }
    }
}
