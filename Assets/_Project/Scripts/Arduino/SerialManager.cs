using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;
using System.Collections.Concurrent;


public class SerialManager : MonoBehaviour
{
    [Header("Seial Setting")]
    [Tooltip("시리얼 통신 기본 세팅 값을 입력하세요.")]
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;
    [SerializeField] private float timeoutLimitSeconds = 0.05f;

    private SerialPort stream;
    private Thread receiveThread;
    private bool isRunning = false;
    private int[] intBuffer = new int[0];
    public static event Action<ArduinoSensorData> OnRawDataReceived;

    private ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

    void Start()
    {
        try
        {
            stream = new SerialPort(portName, baudRate);
            stream.ReadTimeout = (int)(timeoutLimitSeconds * 1000);
            stream.Open();

            isRunning = true;
            receiveThread = new Thread(ReceiveDataWorker);
            receiveThread.Start();

            Debug.Log($"[{portName}] 아두이노 연결 성공, 스레드 시작");
        }

        catch (Exception e)
        {
            Debug.LogError($"아두이노 연결 실패: {e.Message}");
        }
    }

    void Update()
    {
        string[] stringDataToken = null;

        while (dataQueue.TryDequeue(out string localRawData))
        {
            stringDataToken = localRawData.Split(",");
        }

        //Only once when change data element number!
        if (intBuffer.Length != stringDataToken.Length && stringDataToken!=null)
        {
            intBuffer = new int[stringDataToken.Length];
        }

        bool isIntCheckSusses = true;

        for (int i = 0; i < stringDataToken.Length; i++)
        {
            if (!int.TryParse(stringDataToken[i], out intBuffer[i]))
            {
                isIntCheckSusses = false;
                break;
            }
        }

        //Check total try parse string to int
        if (isIntCheckSusses && intBuffer.Length > 0)
        {
            ArduinoSensorData localRawDatas = ArduinoSensorData.ParseData(intBuffer);
            OnRawDataReceived?.Invoke(localRawDatas);

        }
        else
        {
            Debug.LogError("정수 전환 오류: 데이터 토큰을 확인하세요.");
        }
    }

    private void ReceiveDataWorker()
    {
        while (isRunning && stream != null && stream.IsOpen)
        {
            try
            {
                string incomingData = stream.ReadLine();

                if (!string.IsNullOrEmpty(incomingData))
                {
                    dataQueue.Enqueue(incomingData.Trim());
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