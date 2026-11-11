using System;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;

public class ArduinoButtonManager : MonoBehaviour
{
    [SerializeField] private MQTTManager mQTTManager;

    //보정 호출 버튼
    public event Action CalibrationButtonClicked;
    private uint lastCountCalibrationButton = 0;
    private bool isInitializedCalibrationButton = false;

    //맵 위치 변경 버튼
    public event Action MapChangeButtonClicked;
    private uint lastCountMapChangeButton = 0;
    private bool isInitializedMapChangeButton = false;

    //카메라 시점 변경 버튼
    public event Action CameraChangeButtonClicked;
    private uint lastCountCameraChangeButton = 0;
    private bool isInitializedCameraChangeButton = false;

    //Disconnection Detect Flag
    private bool wasDisconnected = false;

    private void OnEnable()
    {
        SensorManager.OnButtonUpdated += ProcessButtonPress;

        if (mQTTManager != null)
        {
            mQTTManager.ChangedMQTTConnection += SetDisconnectionFlag;
        }
    }

    private void OnDisable()
    {
        SensorManager.OnButtonUpdated -= ProcessButtonPress;

        if (mQTTManager != null)
        {
            mQTTManager.ChangedMQTTConnection -= SetDisconnectionFlag;
        }
    }

    private void Start()
    {
        wasDisconnected = false;
    }

    private void SetDisconnectionFlag(ConnectionChanged currentState)
    {
        if (currentState != ConnectionChanged.Connected)
        {
            wasDisconnected = true;
        }

        else return;
    }

    void ProcessButtonPress(ArduinoSensorData targetData)
    {
        //재연결 예외 처리
        if (wasDisconnected)
        {
            wasDisconnected = false;

            lastCountCalibrationButton = targetData.CalibrationButton;
            lastCountMapChangeButton = targetData.MapChangeButton;
            lastCountCameraChangeButton = targetData.CameraChangeButton;

            Debug.Log("[Button] 재연결 직후 전체 버튼 카운트 동기화");
            return;
        }

        //보정 호출 버튼
        ProcessSingleButton(
            targetData.CalibrationButton,
            ref lastCountCalibrationButton,
            ref isInitializedCalibrationButton,
            () => CalibrationButtonClicked?.Invoke(),
            1
            );

        //맵 위치 변경 버튼
        ProcessSingleButton(
            targetData.MapChangeButton,
            ref lastCountMapChangeButton,
            ref isInitializedMapChangeButton,
            () => MapChangeButtonClicked?.Invoke(),
            2
            );

        //카메라 시점 변경 버튼
        ProcessSingleButton(
            targetData.CameraChangeButton,
            ref lastCountCameraChangeButton,
            ref isInitializedCameraChangeButton,
            () => CameraChangeButtonClicked?.Invoke(),
            3
            );
    }

    void ProcessSingleButton(uint currentCount, ref uint lastCount, ref bool isInitialized, Action onClickEvent, int test)
    {
        if (!isInitialized)
        {
            lastCount = currentCount;
            Debug.Log("Flag: 초기 실행 감지 " + lastCount);
            isInitialized = true;
            return;
        }

        if (lastCount > currentCount)
        {
            Debug.Log($"[Button] 재부팅 감지");
            lastCount = currentCount;
            return;
        }

        if (lastCount < currentCount)
        {
            lastCount = currentCount;
            Debug.Log($"{test}번 클릭을 감지했습니다. " + lastCount);
            
            onClickEvent?.Invoke();
        }
    }
}
