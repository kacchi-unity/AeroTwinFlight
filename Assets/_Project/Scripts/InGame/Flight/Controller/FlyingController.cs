using UnityEngine;

enum FlightMode
{
    Absolute,
    Attitude_Control
}

public class FlyingController : MonoBehaviour
{
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private FlightMode flightMode;

    private Quaternion absoluteQuaternion = Quaternion.identity;
    private Quaternion attitudeControlQuaternion = Quaternion.identity;

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
        Quaternion selectedQuaternion = Quaternion.identity;

        switch (flightMode)
        {
            case FlightMode.Absolute:
                selectedQuaternion = this.absoluteQuaternion;
                break;

            case FlightMode.Attitude_Control:
                selectedQuaternion = this.attitudeControlQuaternion;
                break;

            default:
                break;
        }

        targetRigidbody.MoveRotation(selectedQuaternion);
    }
}
