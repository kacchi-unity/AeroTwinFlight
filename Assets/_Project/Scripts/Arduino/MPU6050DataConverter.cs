using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class MPU6050DataConverter : MonoBehaviour
{
    [Header("자동 보정 시간 설정")]
    [SerializeField] private float calibrationTimeSeconds = 3f;

    private WaitForSeconds waitCalibrationDuration;

    private bool isCalibrating = false;

    private List<MPU6050Data> calibrationTargetList = new List<MPU6050Data>();

    private MPU6050OffsetData calibrationOffsetResult;

    private const float accelCoeff = 16384;
    private const float gyroCoeff = 131f;

    public static event Action<MPU6050Data> OnDataReady;

    public static event Action OnCalibrateDone;

    private void OnEnable()
    {
        SensorManager.OnMPUDataUpdated += ProcessMPUData;
        CalibrateManager.onStartCalibrate += CommandProcessCalibrate;
    }

    private void OnDisable()
    {
        SensorManager.OnMPUDataUpdated -= ProcessMPUData;
        CalibrateManager.onStartCalibrate -= CommandProcessCalibrate;
    }

    private void ProcessMPUData(MPU6050Data rawDatas)
    {
        if (isCalibrating)
        {
            calibrationTargetList.Add(rawDatas);
        }

        else
        {
            MPU6050Data calibratedData = new MPU6050Data();

            calibratedData.accelX = (rawDatas.accelX - calibrationOffsetResult.accelX) / accelCoeff;
            calibratedData.accelY = (rawDatas.accelY - calibrationOffsetResult.accelY) / accelCoeff;
            calibratedData.accelZ = ((rawDatas.accelZ - calibrationOffsetResult.accelZ) / accelCoeff) + 1.0f;

            calibratedData.gyroX = (rawDatas.gyroX - calibrationOffsetResult.gyroX) / gyroCoeff;
            calibratedData.gyroY = (rawDatas.gyroY - calibrationOffsetResult.gyroY) / gyroCoeff;
            calibratedData.gyroZ = (rawDatas.gyroZ - calibrationOffsetResult.gyroZ) / gyroCoeff;

            OnDataReady?.Invoke(calibratedData);
        }
    }

    //Zero allocation
    private void Start()
    {
        waitCalibrationDuration = new WaitForSeconds(calibrationTimeSeconds);
    }

    void CommandProcessCalibrate()
    {
        StartCoroutine(CountCalibrateTime());
        Debug.Log($"{calibrationTimeSeconds}초 동안 센서를 움직이지 마세요. 보정 중...");
    }

    IEnumerator CountCalibrateTime()
    {
        calibrationTargetList.Clear();

        isCalibrating = true;
        yield return waitCalibrationDuration;
        CalculateOffsets();
        isCalibrating = false;
    }

    private void CalculateOffsets()
    {
        if (calibrationTargetList.Count == 0)
        {
            Debug.LogWarning("보정 실패: 수집된 데이터를 찾을 수 없음. 연결을 확인하세요.");
            return;
        }

        //1회성 벡터 데이터
        Vector3 accelData = Vector3.zero;
        Vector3 gyroData = Vector3.zero;

        foreach (var data in calibrationTargetList)
        {
            accelData.x += data.accelX;
            accelData.y += data.accelY;
            accelData.z += data.accelZ;

            gyroData.x += data.gyroX;
            gyroData.y += data.gyroY;
            gyroData.z += data.gyroZ;
        }

        int dataNum = calibrationTargetList.Count;

        accelData /= dataNum;
        gyroData /= dataNum;

        calibrationOffsetResult = new MPU6050OffsetData(
            accelData.x,
            accelData.y,
            accelData.z,

            gyroData.x,
            gyroData.y,
            gyroData.z
            );

        isCalibrating = false;
        Debug.Log($"[계산된 가속도 오프셋] x: {accelData.x:F2}, y: {accelData.y:F2}, z: {accelData.z:F2}");
        Debug.Log($"[계산된 자이로 오프셋] X: {gyroData.x:F2}, y: {gyroData.y:F2}, z: {gyroData.z:F2}");

        OnCalibrateDone?.Invoke();

    }
}
