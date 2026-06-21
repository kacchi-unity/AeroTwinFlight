/*using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;

public class ArduinoSerialTest : MonoBehaviour
{
    private SerialPort stream;
    private Thread receiveThread;
    private bool isRunning = false;

    private string rawDataString = "";

    private readonly object lockObject = new object();

    [Header("센서 설정 (Unity 내 계산)")]
    [SerializeField] private const float gCoeff = 131f;
    [SerializeField] private float gyOffset = 0;

    [Header("시리얼 설정")]
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;
    [SerializeField] private float timeoutLimitSeconds = 0.05f;

    void Start()
    {
        try
        {
            //Serial Open
            stream = new SerialPort(this.portName, this.baudRate);
            stream.ReadTimeout = (int)(this.timeoutLimitSeconds * 1000);
            stream.Open();

            //Thread create
            this.isRunning = true;
            receiveThread = new Thread(this.ReceiveDataWorker);
        }

        catch (Exception e)
        {
            Debug.Log($"아두이노 연결 실패: {e.Message}");
        }
    }
}*/
