using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Switch References")]
    public SwitchMaterialChange firstSwitch;
    public SwitchMaterialChange secondSwitch;

    [Header("Required Material Indexes")]
    public int requiredFirstSwitchIndex = 0;
    public int requiredSecondSwitchIndex = 2;

    [Header("Object To Reveal")]
    public GameObject objectToShow;

    [Header("Audio")]
    public SFXAudio sfxAudio;

    [Header("Runtime Info")]
    public bool puzzleSolved = false;

    private bool puzzleSoundPlayed = false;
    private bool lastPuzzleSolvedState = false;

    void Start()
    {
        if (sfxAudio == null)
            sfxAudio = FindFirstObjectByType<SFXAudio>();

        CheckPuzzle(true);
    }

    void Update()
    {
        CheckPuzzle(false);
    }

    void CheckPuzzle(bool forceUpdate)
    {
        if (firstSwitch == null || secondSwitch == null || objectToShow == null)
            return;

        bool correctCombination =
            firstSwitch.currentMaterialIndex == requiredFirstSwitchIndex &&
            secondSwitch.currentMaterialIndex == requiredSecondSwitchIndex;

        puzzleSolved = correctCombination;

        if (forceUpdate || puzzleSolved != lastPuzzleSolvedState)
        {
            if (objectToShow.activeSelf != puzzleSolved)
                objectToShow.SetActive(puzzleSolved);

            lastPuzzleSolvedState = puzzleSolved;
        }

        if (puzzleSolved && !puzzleSoundPlayed)
        {
            puzzleSoundPlayed = true;

            if (sfxAudio != null)
                sfxAudio.PlayPuzzleComplete();
        }

        if (!puzzleSolved)
            puzzleSoundPlayed = false;
    }
}