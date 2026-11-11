using System;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading.Tasks; //비동기 처리
using System.Collections;

public enum ConnectionChanged
{
    Connected,
    MQTTDisconnected,
    MessageDisconnected
}

public class MQTTManager : MonoBehaviour
{
    [Header ("Connection Information")]
    [SerializeField] private string hostIP = "127.0.0.1";
    [SerializeField] private string dataTopics = "flight/sensor";

    [Header("재연결")]
    [SerializeField] private float checkInterval = 2f;
    [SerializeField] private int reconnectionWaitTime = 3;
    [SerializeField] private float firstPacketTimeoutAtWait = 3f;

    private MqttClient mqttClient;
    private bool hasNewMessage = false;
    private byte[] saveByteBuffer;
    private float lastCheckTime;
    private float connectionWaitStartTime;

    public event Action<ConnectionChanged> ChangedMQTTConnection;
    public static event Action<ArduinoSensorData> OnRawDataReceived;

    private enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        WaitingForFirstPacket
    }

    private ConnectionState currentState = ConnectionState.Disconnected;

    void Start()
    {
        hasNewMessage = false;

        currentState = ConnectionState.Disconnected;

        StartCoroutine(ProcessReconnectionRoutine());

        ChangedMQTTConnection?.Invoke(ConnectionChanged.MQTTDisconnected);
    }

    private async Task<bool> ConnectToBrokerAsync()
    {
        bool success = await Task.Run(() =>
        {
            try
            {
                if (mqttClient != null && mqttClient.IsConnected)
                {
                    Debug.Log("이미 연결되어 있어서 재연결 안 함!");
                    return true;
                }

                mqttClient = new MqttClient(hostIP);

                mqttClient.MqttMsgPublishReceived += OnMessageReceived;

                mqttClient.Connect(Guid.NewGuid().ToString());
                mqttClient.Subscribe(new string[] { dataTopics }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

                Debug.Log("MQTT 브로커 연결 및 구독 성공");

                return true;
            }

            catch (Exception)
            {
                Debug.Log("MQTT 브로커 연결 실패");
                return false;
            }
        });

        return success;
    }

    //Sub Thread (Background)
    private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        this.saveByteBuffer = e.Message;
        hasNewMessage = true;
    }

    private IEnumerator ProcessReconnectionRoutine()
    {
        while (currentState != ConnectionState.Connected)
        {
            currentState = ConnectionState.Connecting;

            Debug.Log("MQTT 재연결 시도 호출");
            Task<bool> connectTask = ConnectToBrokerAsync();

            while (!connectTask.IsCompleted)
            {
                yield return null;
            }

            if (connectTask.Result == true)
            {
                currentState = ConnectionState.WaitingForFirstPacket;

                connectionWaitStartTime = Time.time;

                hasNewMessage = false;

                yield break;
            }

            else
            {
                currentState = ConnectionState.Disconnected;
                yield return new WaitForSeconds(this.reconnectionWaitTime);
            }
        }
    }

    void Update()
    {
        if (currentState == ConnectionState.WaitingForFirstPacket)
        {
            if (mqttClient == null || !mqttClient.IsConnected)
            {
                HandleDataTimeout();
                return;
            }

            if (hasNewMessage)
            {
                currentState = ConnectionState.Connected;
                lastCheckTime = Time.time;
                hasNewMessage = false;

                ArduinoSensorData rawDatas = ArduinoSensorData.ParseData(this.saveByteBuffer);
                OnRawDataReceived?.Invoke(rawDatas);

                ChangedMQTTConnection?.Invoke(ConnectionChanged.Connected);
                return;
            }

            if (Time.time - connectionWaitStartTime >= firstPacketTimeoutAtWait)
            {
                HandleDataTimeout();
            }
        }

        else if (currentState == ConnectionState.Connected)
        {
            if (mqttClient == null || !mqttClient.IsConnected)
            {
                HandleDataTimeout();
                return;
            }

            if (hasNewMessage)
            {
                lastCheckTime = Time.time;

                hasNewMessage = false;

                ArduinoSensorData rawDatas = ArduinoSensorData.ParseData(this.saveByteBuffer);

                OnRawDataReceived?.Invoke(rawDatas);
            }

            //주기적 연결 검사
            if ((Time.time - lastCheckTime >= checkInterval))
            {
                HandleDataTimeout();
            }

        }
        
    }

    private void HandleDataTimeout()
    {
        //코루틴 중복 호출 방지
        if (currentState == ConnectionState.Disconnected)
        {
            return;
        }

        //이벤트 1회 호출을 위한 플래그 (lastState가 Connected 인지 검사)
        bool wasConnected = (currentState == ConnectionState.Connected);

        //데이터는 연결이 끊겼으나 MQTT는 살아있는 경우
        if (mqttClient != null && mqttClient.IsConnected)
        {
            if (wasConnected)
            {
                ChangedMQTTConnection?.Invoke(ConnectionChanged.MessageDisconnected);
                Debug.Log("H: 1회 호출");
            }

            Debug.Log("H: MQTT 연결 유지! ESP32 데이터 대기");

            currentState = ConnectionState.WaitingForFirstPacket;

            connectionWaitStartTime = Time.time;
        }

        //MQTT 자체가 끊긴 경우
        else
        {
            Debug.Log("H: MQTT 연결 끊김! 재연결 시작");

            if (wasConnected)
            {
                ChangedMQTTConnection?.Invoke(ConnectionChanged.MQTTDisconnected);
                Debug.Log("H: 1회 호출");
            }

            currentState = ConnectionState.Disconnected;

            StopAllCoroutines();

            StartCoroutine(ProcessReconnectionRoutine());
        }
    }

    private void OnApplicationQuit()
    {
        if (mqttClient != null && mqttClient.IsConnected)
        {
            mqttClient.Disconnect();

            Debug.Log("MQTT 연결을 해제했습니다.");
        }
    }
}
