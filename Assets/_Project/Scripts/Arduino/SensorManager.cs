using System;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public static event Action<short, short> OnMPUDataUpdated;
    public static event Action OnButtonDown;

    private void OnEnable()
    {
        SerialManager.OnRawDataReceived += SendRawData;
    }

    private void OnDisable()
    {
        SerialManager.OnRawDataReceived -= SendRawData;
    }

    void SendRawData(ArduinoSensorData rawDatas)
    {
        OnMPUDataUpdated?.Invoke(rawDatas.gyroX, rawDatas.gyroY);

        if (rawDatas.isButtonPressed)
        {
            OnButtonDown?.Invoke();
        }
    }
}
