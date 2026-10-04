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
        float pitch = selectedQuaternion.eulerAngles.x;

        selectedQuaternion = Quaternion.Euler(
            pitch,
            currentEuler.y,
            currentEuler.z
            );
    

            targetRigidbody.MoveRotation(selectedQuaternion);
    }
}
