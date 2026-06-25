using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using Unity.VisualScripting;

public class MPU6050DataConverter : MonoBehaviour
{
    [Header("센서 설정 (Unity 내 계산)")]
    private const float gCoeff = 131f;
    float gyOffset = 0;
    float gxOffset = 0;

    [Header("자동 보정 설정")]
    [SerializeField] private float calibrationTimeSeconds = 3f;
    private bool isCalibrating = false;
    private List<short> xCalibrationList = new List<short>();
    private List<short> yCalibrationList = new List<short>();
    private WaitForSeconds waitCalibrationDuration;

    [Header("Test")]
    [SerializeField] private Transform t;

    private void OnEnable()
    {
        SensorManager.OnMPUDataUpdated += ProcessMPUData;
    }

    private void OnDisable()
    {
        SensorManager.OnMPUDataUpdated -= ProcessMPUData;
    }

    //Zero allocation
    private void Start()
    {
        waitCalibrationDuration = new WaitForSeconds(calibrationTimeSeconds);
    }

    private void Update() //추후 삭제
    {
       if (Input.GetKeyDown(KeyCode.T))
        {
            CommandProcessCalibrate();
        }
    }

    void CommandProcessCalibrate()
    {
        StartCoroutine(CountCalibrateTime(calibrationTimeSeconds));
        Debug.Log($"{calibrationTimeSeconds}초 동안 센서를 움직이지 마세요. 보정 중...");
    }

    private void ProcessMPUData(short gyroX, short gyroY)
    {
        if (isCalibrating)
        {
            xCalibrationList.Add(gyroX);
            yCalibrationList.Add(gyroY);
        }
        else
        {
            float finalGx = (gyroX - gxOffset) / gCoeff;
            float finalGy = (gyroY - gyOffset) / gCoeff;

            t.Translate(finalGy / 100f, 0, -finalGx / 100f);

            /*Debug.Log(
                $"계산값:\t x축 {finalGx,7:F2}°/s \ty축 {finalGy,7:F2}°/s\t || " +
                $"보정값:\t offset X: {(gxOffset / gCoeff),7:F2}\t offset Y: {(gyOffset / gCoeff),7:F2}"
                );*/
        }
    }

    IEnumerator CountCalibrateTime(float duration)
    {
        xCalibrationList.Clear();
        yCalibrationList.Clear();

        isCalibrating = true;
        yield return new WaitForSeconds(duration);
        CalculateOffsets();
        isCalibrating = false;
    }

    private void CalculateOffsets()
    {
        if (xCalibrationList.Count == 0 && yCalibrationList.Count == 0)
        {
            Debug.LogWarning("보정 실패: 수집된 데이터를 찾을 수 없음. 연결을 확인하세요.");
            return;
        }

        long sumX = 0;
        foreach (short target in xCalibrationList)
        {
            sumX += target;
        }
        gxOffset = (sumX / (float)xCalibrationList.Count);

        long sumY = 0;
        foreach (short target in yCalibrationList)
        {
            sumY += target;
        }
        gyOffset = (sumY / (float)yCalibrationList.Count);

        isCalibrating = false;
        Debug.Log($"구해진 X 오프셋: {gxOffset:F4}, Y 오프셋: {gyOffset:F4}");

    }
}
