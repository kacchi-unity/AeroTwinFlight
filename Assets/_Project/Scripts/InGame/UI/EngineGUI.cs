using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EngineGUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup engineSliderGUI; //To Use Fade In/Out
    [SerializeField] private Slider engineSlider; //To Use Value Change
    [Range(0f, 1f), SerializeField] private float sliderValueDelta = 0.3f;
    [SerializeField] private float fadeOutduration = 1.0f;
    [SerializeField] private float guiDuration = 3.0f;

    private float currentSliderValue = 0f;
    private float lastSliderValue;
    private Coroutine currentFadeCoroutine;
    private WaitForSeconds guiWaitTime;

    private void OnEnable()
    {
        SensorManager.OnSliderValueUpdated += SaveSliderValue;

        guiWaitTime = new WaitForSeconds(guiDuration);
    }

    private void OnDisable()
    {
        SensorManager.OnSliderValueUpdated -= SaveSliderValue;
    }

    private void SaveSliderValue(float targetSlider)
    {
        this.currentSliderValue = targetSlider;
    }

    private void Start()
    {
        //Slider
        this.engineSlider.value = this.currentSliderValue;

        //Local Value, GUI
        this.lastSliderValue = this.currentSliderValue;

        if (engineSliderGUI != null)
        {
            engineSliderGUI.alpha = 0f;
        }
    }

    private void Update()
    {
        this.engineSlider.value = this.currentSliderValue;

        if (Mathf.Abs(currentSliderValue - lastSliderValue) >= sliderValueDelta)
        {
            lastSliderValue = currentSliderValue;

            ResetTimerAndShow();
        }
    }

    private void ResetTimerAndShow()
    {
        this.engineSliderGUI.alpha = 1f;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }

        currentFadeCoroutine = StartCoroutine(WaitAndFadeOutRoutine(engineSliderGUI, 1, 0, fadeOutduration, guiDuration));
    }

    private IEnumerator WaitAndFadeOutRoutine(CanvasGroup target, float startAlpha, float endAlpha, float fadeOutduration, float guiDuration)
    {
        yield return this.guiWaitTime;

        float timer = 0f;

        while (timer < fadeOutduration)
        {
            timer += Time.deltaTime;
            target.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeOutduration);
            yield return null;
        }

        engineSliderGUI.alpha = endAlpha;

        this.lastSliderValue = this.currentSliderValue;
    }
}
