using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;
using System.Collections.Generic;


public class SerialManager : MonoBehaviour
{
    [Header("Seial Setting")]
    [Tooltip("시리얼 통신 기본 세팅 값을 입력하세요.")]
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;
    [SerializeField] private float timeoutLimitSeconds = 0.05f;

    [Header("패킷 프로토콜 번호 (아두이노와 일치 필수)")]
    [Tooltip("아두이노에서 설정한 패킷 헤더(Header)와 트레일러(Trailer)를 입력하세요(1Byte).")]
    [SerializeField] private byte firstHeader = 0xAA;
    [SerializeField] private byte secondHeader = 0xBB;
    [SerializeField] private byte firstTrailer = 0xCC;
    [SerializeField] private byte secondTrailer = 0xDD;

    public static event Action<ArduinoSensorData> OnRawDataReceived;

    private SerialPort stream;
    private Thread receiveThread;
    private bool isRunning = false;

    //ConcurrentQueue 대신 일반 Queue, Lock으로 통제
    private Queue<byte> dataQueue = new Queue<byte>();
    private readonly object queueLock = new object();

    private byte[] readBuffer = new byte[256];
    private int[] intBuffer = new int[0];
    private byte[] saveArray = new byte[256];

    void Start()
    {
        ArduinoSensorData tempStruct = new ArduinoSensorData();
        intBuffer = new int[tempStruct.GetTotalDataCount()];

        try
        {
            stream = new SerialPort(portName, baudRate);
            stream.ReadTimeout = (int)(timeoutLimitSeconds * 1000);
            stream.ReadBufferSize = 8192; //OS 버퍼 확장
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
        while (true)
        {
            int queueCount = 0;

            lock (queueLock)
            {
                queueCount = dataQueue.Count;
            }

            if (queueCount < 5) //Header 2 + Size 1 + Trailer 2
            {
                break;
            }

            bool isHeaderValid = false;
            int packetSize = 0;

            //lock 내 일반 Queue 3비트 유효성 검사, 헤더2 + 크기1
            lock (queueLock)
            {
                var enumerator = dataQueue.GetEnumerator();
                if (enumerator.MoveNext() && enumerator.Current == firstHeader)
                {
                    if (enumerator.MoveNext() && enumerator.Current == secondHeader)
                    {
                        if (enumerator.MoveNext())
                        {
                            packetSize = enumerator.Current;
                            isHeaderValid = true;
                        }
                    }
                }
            }

            if (isHeaderValid)
            {
                int totalPacketLength = 2 + 1 + packetSize + 2;

                if (queueCount < totalPacketLength)
                {
                    //패킷이 덜 채워짐 - 다음 프레임으로
                    break;
                }

                //Trailer 유효성 검사
                bool isTrailerValid = false;

                lock (queueLock)
                {
                    var enumerator = dataQueue.GetEnumerator();
                    for (int i = 0; i < totalPacketLength - 1; i++)
                    {
                        enumerator.MoveNext();
                    }

                    byte trailer1 = enumerator.Current;
                    enumerator.MoveNext();
                    byte trailer2 = enumerator.Current;

                    if (trailer1 == firstTrailer && trailer2 == secondTrailer)
                    {
                        isTrailerValid = true;
                    }

                }//패킷 Header, Trailer 모두 검사 완료

                //Header, Trailer 모두 일치: 완벽한 패킷 구조 인식
                if (isTrailerValid)
                {
                    lock (queueLock)
                    {
                        for (int i = 0; i < totalPacketLength; i++)
                        {
                            saveArray[i] = dataQueue.Dequeue();
                        }
                    } //lock 종료: 내부에선 Enqueue, Dequeue만 처리

                    int dataStartIndex = 3; //헤더2 + 사이즈1
                    for (int i = 0; i < intBuffer.Length; i++)
                    {
                        int byteIndex = dataStartIndex + (i * 2);

                        //Little Endian: Window, 자동 조립 (Low -> High)
                        //BitConverter: PC환경에 맞춰 리틀 엔디안으로 자동 조립
                        intBuffer[i] = BitConverter.ToInt16(saveArray, byteIndex);
                    }

                    ArduinoSensorData localRawDatas = ArduinoSensorData.ParseData(intBuffer);
                    OnRawDataReceived?.Invoke(localRawDatas);
                }

                else //Trailer 틀림: 1 Byte만 버림
                {
                    lock (queueLock)
                    {
                        dataQueue.Dequeue();
                    }
                }

            }//if (isHeaderValid)

            else //Header 틀림: 1 Byte만 버림
            {
                lock (queueLock)
                {
                    dataQueue.Dequeue();
                }
            }

        }//while
    } //Update()

    private void ReceiveDataWorker()
    {
        while (isRunning && stream != null && stream.IsOpen)
        {
            try
            {
                // Serial Buffer 내 바이트 개수만 확인 (n Byte)
                int bytesCountToSerialBuffer = stream.BytesToRead;
                if (bytesCountToSerialBuffer > 0)
                {
                    // 버퍼 크기 방어 코드
                    int count = Math.Min(bytesCountToSerialBuffer, readBuffer.Length);

                    // Read(저장할 바이트 배열, 배열의 저장 시작 인덱스, 읽을 바이트 최대 개수)
                    // int: 실제로 읽어온 바이트 수 리턴
                    int bytesCountToRead = stream.Read(readBuffer, 0, count);

                    lock (queueLock)
                    {
                        for (int i = 0; i < bytesCountToRead; i++)
                        {
                            dataQueue.Enqueue(readBuffer[i]);
                        }
                    }
                }
                else
                {
                    // 데이터가 안들어올 때 1ms 휴식 (발열 방지)
                    Thread.Sleep(1);
                }
            }

            catch (TimeoutException)
            {
                //Pass
            }

            catch (Exception e)
            {
                System.Console.WriteLine($"스레드 수신 에러: {e.Message}");
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