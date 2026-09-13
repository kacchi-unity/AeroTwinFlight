using UnityEngine;
using System;
using TMPro;

public class CalibrateManager : MonoBehaviour
{
    public static event Action onStartCalibrate;
    public static event Action onFinishCalibrate;

    [Header("Calibrate State Message")]
    [SerializeField] private TextMeshProUGUI calibrateMessageTMP;
    [SerializeField] private string beforeCalibrateMessage;
    [SerializeField] private string processingCalibrateMessage;
    [SerializeField] private string afterCalibrateMessage;

    private void OnEnable()
    {
        InGameButtonManager.onCalibrateButtonClicked += ProcessCalibrationStart;
        MPU6050DataConverter.OnCalibrateDone += ProcessCalibrationFinish;
    }

    private void OnDisable()
    {
        InGameButtonManager.onCalibrateButtonClicked -= ProcessCalibrationStart;
        MPU6050DataConverter.OnCalibrateDone -= ProcessCalibrationFinish;
    }

    void Start()
    {
        calibrateMessageTMP.text = beforeCalibrateMessage;
    }

    void ProcessCalibrationStart()
    {
        onStartCalibrate?.Invoke();
        calibrateMessageTMP.text = processingCalibrateMessage;

    }

    void ProcessCalibrationFinish()
    {
        onFinishCalibrate?.Invoke();
        calibrateMessageTMP.text = afterCalibrateMessage;
    }
}
