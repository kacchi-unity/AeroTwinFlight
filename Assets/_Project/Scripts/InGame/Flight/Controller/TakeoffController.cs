using UnityEngine;

enum TaxiingFlightMode
{
    Absolute,
    Attitude_Control
}

public class TakeoffController : MonoBehaviour
{
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private TaxiingFlightMode flightMode;

    private Quaternion absoluteQuaternion = Quaternion.identity;
    private Quaternion attitudeControlQuaternion = Quaternion.identity;

    private float pitchOffset = 0f;

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

    //진입 순간 pitch 현재 기울기를 오프셋으로 저장
    public void InitializeOffset()
    {
        Quaternion currenQuaternion = (flightMode == TaxiingFlightMode.Absolute) ?
                                   absoluteQuaternion : 
                                   attitudeControlQuaternion ;

        float sensorPitch = currenQuaternion.eulerAngles.x;
        float currentPlanePitch = targetRigidbody.rotation.eulerAngles.x;

        // 현재 지면(경사면) 상태와 센서 값의 차이를 기억
        pitchOffset = currentPlanePitch - sensorPitch;
    }

    //Use Data
    public void UpdateRotation()
    {
        ProcessRotation();
    }

    private void ProcessRotation()
    {
        Quaternion selectedQuaternion = Quaternion.identity;

        switch (flightMode)
        {
            case TaxiingFlightMode.Absolute:
                selectedQuaternion = this.absoluteQuaternion;
                break;

            case TaxiingFlightMode.Attitude_Control:
                selectedQuaternion = this.attitudeControlQuaternion;
                break;

            default:
                break;
        }


        Vector3 currentEuler = targetRigidbody.rotation.eulerAngles;
        float sensorPitch = selectedQuaternion.eulerAngles.x;

        //초기 offset 적용
        float finalPitch = sensorPitch + pitchOffset;

        selectedQuaternion = Quaternion.Euler(
            finalPitch,
            currentEuler.y,
            currentEuler.z
            );
    

            targetRigidbody.MoveRotation(selectedQuaternion);
    }
}
