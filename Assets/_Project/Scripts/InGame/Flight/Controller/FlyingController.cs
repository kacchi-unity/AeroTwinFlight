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

    private Quaternion absoluteQuaternion = Quaternion.identity;
    private Quaternion attitudeControlQuaternion = Quaternion.identity;

    //진입 순간 회전값 저장
    private Quaternion entryAircraftRotation;
    private Quaternion entryAbsoluteSensorRotation;
    private Quaternion entryAttitudeSensorRotation;

    private bool hasRotationReference = false;

    public void InitializeRotationReferecne()
    {
        entryAircraftRotation = targetRigidbody.rotation;

        entryAbsoluteSensorRotation = absoluteQuaternion;

        entryAttitudeSensorRotation = attitudeControlQuaternion;

        hasRotationReference = true;
    }

    private void OnEnable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated += SaveAbsoluteQuaternion;
        SensorQuaternionCalculator.OnAttitudeControlQuaternionCalculated += SaveAttitudeControlQuaternion;
    }

    private void OnDisable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated -= SaveAbsoluteQuaternion;
        SensorQuaternionCalculator.OnAttitudeControlQuaternionCalculated -= SaveAttitudeControlQuaternion;
    }

    //Save Data
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
        if (!hasRotationReference)
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

        Quaternion finalRotation = entryAircraftRotation * relativeRotation;

        targetRigidbody.MoveRotation(finalRotation);
    }
}
