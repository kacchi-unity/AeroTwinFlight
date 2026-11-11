using System;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public static event Action<MPU6050Data> OnMPUDataUpdated;
    public static event Action<ArduinoSensorData> OnButtonUpdated;
    public static event Action<float> OnSliderValueUpdated;

    private void OnEnable()
    {
        SerialManager.OnRawDataReceived += SplitAndDistributeRawData;
        MQTTManager.OnRawDataReceived += SplitAndDistributeRawData;
    }

    private void OnDisable()
    {
        SerialManager.OnRawDataReceived -= SplitAndDistributeRawData;
        MQTTManager.OnRawDataReceived -= SplitAndDistributeRawData;
    }

    void SplitAndDistributeRawData(ArduinoSensorData totalRawDatas)
    {
        MPU6050Data mpuDatas = new MPU6050Data(totalRawDatas);
        OnMPUDataUpdated?.Invoke(mpuDatas);

        OnButtonUpdated?.Invoke(totalRawDatas);

        OnSliderValueUpdated?.Invoke(totalRawDatas.SliderValue);
    }
}
