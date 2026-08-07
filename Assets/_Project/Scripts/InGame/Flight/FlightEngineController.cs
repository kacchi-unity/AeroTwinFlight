using UnityEngine;

public class FlightEngineController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private float enginePower = 2;
    [SerializeField, Range(0f, 1f)] private float velocityAlignmentRate = 0.05f;
    [SerializeField] private float minVelocitySqrThreshold = 0.1f;

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

            if (flightRigidbody.linearVelocity.sqrMagnitude > minVelocitySqrThreshold)
            {
                Vector3 alignedVelocity = transform.forward * flightRigidbody.linearVelocity.magnitude;
                flightRigidbody.linearVelocity = Vector3.Lerp(flightRigidbody.linearVelocity, alignedVelocity, velocityAlignmentRate);
            }
        }
    }
}
