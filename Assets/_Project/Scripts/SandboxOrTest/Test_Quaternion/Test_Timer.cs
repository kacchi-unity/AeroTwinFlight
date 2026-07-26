using TMPro;
using UnityEngine;

public class Test_Timer : MonoBehaviour
{
    bool isTimerStart = false;
    float totalTime = 0f;
    [SerializeField] private TextMeshProUGUI timerText;

    private void OnEnable()
    {
        CalibrateManager.onFinishCalibrate += StartTimer;
    }

    private void OnDisable()
    {
        CalibrateManager.onFinishCalibrate -= StartTimer;
    }

    void StartTimer()
    {
        isTimerStart = true;
    }

    void Update()
    {
        if (isTimerStart)
        {
            totalTime += Time.deltaTime;
            int totalSec = (int)totalTime;
            int minite = totalSec / 60;
            int second = totalSec % 60;

            timerText.text = $"{minite:00}:{second:00}";
        }
    }
}
