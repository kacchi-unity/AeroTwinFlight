using UnityEngine;
using System.Collections;

public class TakeoffGuideGUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup takeoffGuideGUI;
    [Range(0f, 1f), SerializeField] private float maxAlpha = 1f;
    [Range(0f, 1f), SerializeField] private float minAlpha = 0.5f;
    [SerializeField] private float blinkInterval = 1f;

    private Coroutine currentBlinkCoroutine;

    private void OnDisable()
    {
        currentBlinkCoroutine = null;
    }

    private void Start()
    { 
        if (takeoffGuideGUI != null)
        {
            takeoffGuideGUI.alpha = 0f;
        }
    }

    public void OnBlinkGUI()
    {
        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
        }

        currentBlinkCoroutine = StartCoroutine(BlinkRoutine(takeoffGuideGUI, maxAlpha, minAlpha, blinkInterval));
    }

    public void OffBlinkGUI()
    {
        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
            currentBlinkCoroutine = null;
        }

        if (takeoffGuideGUI != null)
        {
            takeoffGuideGUI.alpha = 0f;
        }
    }

    private IEnumerator BlinkRoutine(CanvasGroup target, float startAlpha, float endAlpha, float interval)
    {
        if (target == null)
        {
            yield break;
        }

        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            float progress = Mathf.PingPong(timer, interval) / interval;

            target.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

            yield return null;
        }
    }
}
