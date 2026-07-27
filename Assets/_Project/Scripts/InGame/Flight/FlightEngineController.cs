using UnityEngine;

public class FlightEngineController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private float enginePower = 500;

    bool isEngineAllowed = false;

    private void OnEnable()
    {
        CalibrateManager.onFinishCalibrate += EnableEnginePower;
    }

    private void OnDisable()
    {
        CalibrateManager.onFinishCalibrate -= EnableEnginePower;
    }
    
    void EnableEnginePower()
    {
        isEngineAllowed = true;
    }

    private void FixedUpdate()
    {
        if (isEngineAllowed)
        {
            flightRigidbody.AddRelativeForce(Vector3.forward * enginePower);
        }
    }
}
