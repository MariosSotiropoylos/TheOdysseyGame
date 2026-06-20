using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("Main Pause Menu")]
    public GameObject pauseMenu;

    [Header("Click This UI To Close Everything")]
    public GameObject closePauseUI;

    [Header("Extra UI Items To Close When Leaving Pause")]
    public GameObject[] extraUIItems = new GameObject[7];

    [Header("State")]
    public bool isPaused = false;

    void Start()
    {
        SetupCloseUI();
        ClosePauseMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            OpenPauseMenu();
        }
    }

    void SetupCloseUI()
    {
        if (closePauseUI == null)
        {
            return;
        }

        Button button = closePauseUI.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ClosePauseMenu);
            return;
        }

        EventTrigger eventTrigger = closePauseUI.GetComponent<EventTrigger>();

        if (eventTrigger == null)
        {
            eventTrigger = closePauseUI.AddComponent<EventTrigger>();
        }

        eventTrigger.triggers.Clear();

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;

        clickEntry.callback.AddListener((eventData) =>
        {
            ClosePauseMenu();
        });

        eventTrigger.triggers.Add(clickEntry);
    }

    public void OpenPauseMenu()
    {
        isPaused = true;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }

        Time.timeScale = 0f;

        // Pauses all normal AudioSources
        AudioListener.pause = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePauseMenu()
    {
        isPaused = false;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        for (int i = 0; i < extraUIItems.Length; i++)
        {
            if (extraUIItems[i] != null)
            {
                extraUIItems[i].SetActive(false);
            }
        }

        Time.timeScale = 1f;

        // Resumes all normal AudioSources.
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}