using System;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public static event Action<MPU6050Data> OnMPUDataUpdated;
    public static event Action OnButtonDown;

    private void OnEnable()
    {
        SerialManager.OnRawDataReceived += SplitAndDistributeRawData;
    }

    private void OnDisable()
    {
        SerialManager.OnRawDataReceived -= SplitAndDistributeRawData;
    }

    void SplitAndDistributeRawData(ArduinoSensorData totalRawDatas)
    {
        MPU6050Data mpuDatas = new MPU6050Data(totalRawDatas);
        OnMPUDataUpdated?.Invoke(mpuDatas);

        if (totalRawDatas.isButtonPressed)
        {
            OnButtonDown?.Invoke();
        }
    }
}
