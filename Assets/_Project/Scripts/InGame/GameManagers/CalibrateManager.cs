using UnityEngine;
using System;
using TMPro;

public class CalibrateManager : MonoBehaviour
{
    [Header ("Manager Connection")]
    [SerializeField] private ArduinoButtonManager arduinoButtonManager;

    public static event Action onStartCalibrate;
    public static event Action onFinishCalibrate;

    //flag (중복 호출 방지)
    private bool isCalibrationInProgress = false;

    [Header("Calibrate State Message")]
    [SerializeField] private TextMeshProUGUI calibrateMessageTMP;
    [SerializeField] private string beforeCalibrateMessage;
    [SerializeField] private string processingCalibrateMessage;
    [SerializeField] private string afterCalibrateMessage;

    private void OnEnable()
    {
        InGameButtonManager.onCalibrateButtonClicked += ProcessCalibrationStart;
        MPU6050DataConverter.OnCalibrateDone += ProcessCalibrationFinish;
        arduinoButtonManager.CalibrationButtonClicked += ProcessCalibrationStart;
    }

    private void OnDisable()
    {
        InGameButtonManager.onCalibrateButtonClicked -= ProcessCalibrationStart;
        MPU6050DataConverter.OnCalibrateDone -= ProcessCalibrationFinish;
        arduinoButtonManager.CalibrationButtonClicked -= ProcessCalibrationStart;
    }

    void Start()
    {
        calibrateMessageTMP.text = beforeCalibrateMessage;
    }

    void ProcessCalibrationStart()
    {
        if (!isCalibrationInProgress)
        {
            isCalibrationInProgress = true;
            onStartCalibrate?.Invoke();
            calibrateMessageTMP.text = processingCalibrateMessage;
        }
    }

    void ProcessCalibrationFinish()
    {
        onFinishCalibrate?.Invoke();
        calibrateMessageTMP.text = afterCalibrateMessage;
        isCalibrationInProgress = false;
    }
}
