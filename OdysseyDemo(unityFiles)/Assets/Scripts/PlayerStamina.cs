using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("STAMINA SETTINGS")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("STAMINA DRAIN / REGEN")]
    public float staminaDrainPerSecond = 20f;
    public float staminaRegenPerSecond = 10f;
    public float regenDelayAfterRunning = 1.0f;

    [Header("RUNNING RULES")]
    public float minimumStaminaToRun = 10f;

    [Header("UI")]
    public Slider staminaSlider;
    public CanvasGroup staminaCanvasGroup;

    [Header("UI VISIBILITY")]
    public bool showOnlyWhenRunning = true;
    public float fadeSpeed = 20f;

    [Header("RUNTIME INFO")]
    public bool isRunning = false;
    public bool isExhausted = false;

    private float lastTimeStaminaWasUsed = -999f;

    void Start()
    {
        currentStamina = maxStamina;

        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;

            if (staminaCanvasGroup == null)
            {
                staminaCanvasGroup = staminaSlider.GetComponent<CanvasGroup>();

                if (staminaCanvasGroup == null)
                {
                    staminaCanvasGroup = staminaSlider.gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        UpdateStaminaUI();
        UpdateStaminaVisibility(true);
    }

    void Update()
    {
        RegenerateStamina();
        UpdateStaminaUI();
        UpdateStaminaVisibility(false);
    }

    public bool HandleRunning(bool wantsToRun, float deltaTime)
    {
        if (wantsToRun && CanRun())
        {
            DrainStamina(deltaTime);
            isRunning = true;
            return true;
        }

        isRunning = false;
        return false;
    }

    bool CanRun()
    {
        if (currentStamina <= 0f)
        {
            isExhausted = true;
            return false;
        }

        if (isExhausted && currentStamina < minimumStaminaToRun)
        {
            return false;
        }

        isExhausted = false;
        return true;
    }

    void DrainStamina(float deltaTime)
    {
        currentStamina -= staminaDrainPerSecond * deltaTime;

        if (currentStamina < 0f)
        {
            currentStamina = 0f;
        }

        lastTimeStaminaWasUsed = Time.time;
    }

    void RegenerateStamina()
    {
        if (isRunning)
        {
            return;
        }

        if (Time.time < lastTimeStaminaWasUsed + regenDelayAfterRunning)
        {
            return;
        }

        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenPerSecond * Time.deltaTime;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }

        if (currentStamina >= minimumStaminaToRun)
        {
            isExhausted = false;
        }
    }

    void UpdateStaminaUI()
    {
        if (staminaSlider == null)
        {
            return;
        }

        staminaSlider.value = currentStamina;
    }

    void UpdateStaminaVisibility(bool instant)
    {
        if (staminaCanvasGroup == null)
        {
            return;
        }

        float targetAlpha = 1f;

        if (showOnlyWhenRunning)
        {
            targetAlpha = isRunning ? 1f : 0f;
        }

        if (instant)
        {
            staminaCanvasGroup.alpha = targetAlpha;
        }
        else
        {
            staminaCanvasGroup.alpha = Mathf.Lerp(
                staminaCanvasGroup.alpha,
                targetAlpha,
                fadeSpeed * Time.deltaTime
            );
        }

        staminaCanvasGroup.interactable = false;
        staminaCanvasGroup.blocksRaycasts = false;
    }

    public void ResetStamina()
    {
        currentStamina = maxStamina;
        isRunning = false;
        isExhausted = false;

        UpdateStaminaUI();
        UpdateStaminaVisibility(true);
    }
}