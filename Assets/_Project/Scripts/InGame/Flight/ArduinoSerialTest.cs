using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Drawing.Text;

// ====================================================================
// [아두이노 통신 프로토콜 규칙]
// 아두이노 전송 형태: "데이터1,데이터2, ... \n" (예: "512,1\n")
// 
// index [0] : 자이로 x (int), x축 각속도
// index [1] : 자이로 y (int), y축 각속도
// index [2] : 버튼 데이터 (int)
// ====================================================================

public class ArduinoSerialTest : MonoBehaviour
{
    private SerialPort stream;
    private Thread receiveThread;
    private bool isRunning = false;

    private string rawDataString = "";

    private readonly object lockObject = new object();

    [Header("센서 설정 (Unity 내 계산)")]
    private const float gCoeff = 131f;
    float gyOffset = 0;
    float gxOffset = 0;
    
    
    [Header("시리얼 설정")]
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;
    [SerializeField] private float timeoutLimitSeconds = 0.05f;

    [SerializeField] private Transform t;

    [Header("자동 보정 설정")]
    [SerializeField] private float calibrationTimeSeconds = 3f;
    private bool isCalibrating = true;
    private float calibrationTimer = 0;
    private List<int> xCalibrationList = new List<int>();
    private List<int> yCalibrationList = new List<int>();

    //Button Signal
    private int lastButtonState = 0;

    void Start()
    {
        try
        {
            stream = new SerialPort(portName, baudRate);
            stream.ReadTimeout = (int)(timeoutLimitSeconds*1000);
            stream.Open();

            isRunning = true;
            receiveThread = new Thread(ReceiveDataWorker);
            receiveThread.Start();

            Debug.Log($"[{portName}] 아두이노 연결 성공, 스레드 시작");
            Debug.Log("3초 동안 센서를 움직이지 마세요. 보정 중...");
        }

        catch (Exception e)
        {
            Debug.LogError($"아두이노 연결 실패: {e.Message}");
        }
    }

    void Update() //추후 삭제
    {
        if (isCalibrating)
        {
            calibrationTimer += Time.deltaTime;

            if (calibrationTimer >= calibrationTimeSeconds)
            {
                CalculateOffsets();
            }
        }

        string localRawData = null;

        lock (lockObject)
        {
            if (!string.IsNullOrEmpty(rawDataString))
            {
                localRawData = rawDataString;
                rawDataString = "";
            }
        }

        if (localRawData != null)
        {
            string[] datas = localRawData.Split(",");
            
            if (datas.Length == 3)
            {
                if (int.TryParse(datas[0], out int rawGx) && int.TryParse(datas[1], out int rawGy))
                {
                    if (isCalibrating)
                    {
                        xCalibrationList.Add(rawGx);
                        yCalibrationList.Add(rawGy);
                    }
                    else
                    {
                        float finalGx = (rawGx - gxOffset) / gCoeff;
                        float finalGy = (rawGy - gyOffset) / gCoeff;

                        t.Translate(finalGy / 100f, 0, -finalGx / 100f);

                        //Debug.Log($"[보정 완료]계산값: x축 {finalGx:F2}°/s, y축 {finalGy:F2}°/s");
                    }
                }

                if (int.TryParse(datas[2], out int buttonState))
                {
                    HandleButtonData(buttonState);
                }
            }
        }
    }

    private void CalculateOffsets()
    {
        Debug.Log("Test2");
        long sumX = 0;
        foreach (int target in xCalibrationList)
        {
            sumX += target;
        }
        gxOffset = (sumX / (float)xCalibrationList.Count);

        long sumY = 0;
        foreach (int target in yCalibrationList)
        {
            sumY += target;
        }
        gyOffset = (sumY / (float)yCalibrationList.Count);

        isCalibrating = false;
        Debug.Log($"구해진 X 오프셋: {gxOffset:F4}, Y 오프셋: {gyOffset:F4}");

    }

    void HandleButtonData(int currentButtonState)
    {
        if (currentButtonState == 1 && lastButtonState == 0)
        {
            Debug.Log("버튼 클릭 감지 = 1");
        }

        lastButtonState = currentButtonState;
    }

    private void ReceiveDataWorker()
    {
        while (isRunning && stream != null && stream.IsOpen)
        {
            try
            {
                string incomingData = stream.ReadLine();

                //Avoid serial buffer overflow
                while (stream.BytesToRead > 0)
                {
                    incomingData = stream.ReadLine();
                }

                if (!string.IsNullOrEmpty(incomingData))
                {
                    lock (lockObject)
                    {
                        rawDataString = incomingData.Trim();
                    }
                }
            }
            catch (TimeoutException)
            {
                //wait pass
            }

            catch (Exception e)
            {
                Debug.Log($"스레드 수신 에러 {e.Message}");
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join();
        }

        if (stream != null && stream.IsOpen)
        {
            stream.Close();
            Debug.Log("시리얼 포트를 닫았습니다.");
        }
    }

}