using UnityEngine;

enum FlyingFlightMode
{
    Absolute,
    Attitude_Control
}

public class FlyingController : MonoBehaviour
{
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private FlyingFlightMode flightMode;

    [Header ("연결 끊김 자동 비행 모드 관리 오브젝트")]
    [SerializeField] private MQTTManager mQTTManager;

    private Quaternion absoluteQuaternion = Quaternion.identity;
    private Quaternion attitudeControlQuaternion = Quaternion.identity;

    //진입 순간 회전값 저장
    private Quaternion entryFlightRotation;
    private Quaternion entryAbsoluteSensorRotation;
    private Quaternion entryAttitudeSensorRotation;

    private bool hasRotationReference = false;
    private bool isRotationAllowed = true;

    //To Calibration
    private Vector3 savedVelocity;
    private Vector3 savedAngularVelocity;

    public void InitializeRotationReference()
    {
        entryFlightRotation = targetRigidbody.rotation;

        entryAbsoluteSensorRotation = absoluteQuaternion;

        entryAttitudeSensorRotation = attitudeControlQuaternion;

        hasRotationReference = true;
    }

    private void OnEnable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated += SaveAbsoluteQuaternion;
        SensorQuaternionCalculator.OnAttitudeControlQuaternionCalculated += SaveAttitudeControlQuaternion;

        if (mQTTManager != null)
        {
            mQTTManager.ChangedMQTTConnection += EnterFailSafe;
        }
    }

    private void OnDisable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated -= SaveAbsoluteQuaternion;
        SensorQuaternionCalculator.OnAttitudeControlQuaternionCalculated -= SaveAttitudeControlQuaternion;

        if (mQTTManager != null)
        {
            mQTTManager.ChangedMQTTConnection -= EnterFailSafe;
        }
    }

    //Disconnection's Auto Failsafe
    void EnterFailSafe(ConnectionChanged targetState)
    {
        if (targetState != ConnectionChanged.Connected)
        {
            isRotationAllowed = false;

            Vector3 currentEuler = targetRigidbody.rotation.eulerAngles;

            Quaternion failSafeRotation = Quaternion.Euler(0f, currentEuler.y, 0f);

            targetRigidbody.MoveRotation(failSafeRotation);
        }

        else
        {
            isRotationAllowed = true;
        }
    }


    //Calibrating Physics
    public void StartCalibration()
    {
        SaveAndFreezeRigidbody();
    }

    public void FinishCalibration()
    {
        LoadAndUnFreezeRigidbody();
    }

    private void SaveAndFreezeRigidbody()
    {
        savedVelocity = targetRigidbody.linearVelocity;
        savedAngularVelocity = targetRigidbody.angularVelocity;
        targetRigidbody.isKinematic = true;
    }

    private void LoadAndUnFreezeRigidbody()
    {
        targetRigidbody.isKinematic = false;
        targetRigidbody.linearVelocity = savedVelocity;
        targetRigidbody.angularVelocity = savedAngularVelocity;
    }

    //Save Local Data
    void SaveAbsoluteQuaternion(Quaternion quaternionData)
    {
        this.absoluteQuaternion = quaternionData;
    }

    void SaveAttitudeControlQuaternion(Quaternion quaternionData)
    {
        this.attitudeControlQuaternion = quaternionData;
    }

    //Use Data
    public void UpdateRotation()
    {
        ProcessRotation();
    }

    private void ProcessRotation()
    {
        if (!hasRotationReference || !isRotationAllowed)
        {
            return;
        }

        Quaternion sensorRotation = Quaternion.identity;
        Quaternion entrySensorRotation = Quaternion.identity;

        switch (flightMode)
        {
            case FlyingFlightMode.Absolute:
                sensorRotation = this.absoluteQuaternion;
                entrySensorRotation = entryAbsoluteSensorRotation;
                break;

            case FlyingFlightMode.Attitude_Control:
                sensorRotation = this.attitudeControlQuaternion;
                entrySensorRotation = entryAttitudeSensorRotation;
                break;

            default:
                break;
        }

        //State 진입 이후 센서가 변화한 상대적 회전량 (ex. inverse(20)*35 = 15)
        Quaternion relativeRotation = Quaternion.Inverse(entrySensorRotation) * sensorRotation;

        Quaternion finalRotation = entryFlightRotation * relativeRotation;

        targetRigidbody.MoveRotation(finalRotation);
    }
}
