using System;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading.Tasks; //비동기 처리

public class MQTTManager : MonoBehaviour
{
    [SerializeField] private string hostIP = "127.0.0.1";
    [SerializeField] private string dataTopics = "flight/sensor";

    private MqttClient mqttClient;
    private bool hasNewMessage = false;
    private byte[] saveByteBuffer;
    private bool isConnectionFailed = false;
    private string errorMessage;

    public static event Action<ArduinoSensorData> OnRawDataReceived;

    void Start()
    {
        Task.Run(ConnectToBrokerAsync);
    }

    private void ConnectToBrokerAsync()
    {
        try
        {
            mqttClient = new MqttClient(hostIP);

            mqttClient.MqttMsgPublishReceived += OnMessageReceived;

            mqttClient.Connect(Guid.NewGuid().ToString());
            mqttClient.Subscribe(new string[] { dataTopics }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

            Debug.Log("MQTT 브로커 연결 및 구독 성공");
        }

        catch (Exception e)
        {
            isConnectionFailed = true;
            errorMessage = e.Message;
        }
    }

    //Sub Thread (Background)
    void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        this.saveByteBuffer = e.Message;
        hasNewMessage = true;
    }

    void Update()
    {
        if (hasNewMessage)
        {
            hasNewMessage = false;

            ArduinoSensorData rawDatas = ArduinoSensorData.ParseData(this.saveByteBuffer);
            
            OnRawDataReceived?.Invoke(rawDatas);
        }

        if (isConnectionFailed)
        {
            isConnectionFailed = false;
            Debug.LogError($"MQTT 연결 실패: {errorMessage}");
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
