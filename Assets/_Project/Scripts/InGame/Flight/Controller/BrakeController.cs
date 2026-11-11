using UnityEngine;
using UnityEngine.UI;

public class BrakeController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private float brakeForce = 3000f;

    float currentEngineThrottle = 0f;
    private bool isbrakeEnabled = true;
    private float currentBrakeForce = 0; //For UI Print

    private void OnEnable()
    {
        SensorManager.OnSliderValueUpdated += SaveSliderValue;
    }

    private void OnDisable()
    {
        SensorManager.OnSliderValueUpdated -= SaveSliderValue;
    }

    private void SaveSliderValue(float currentSliderValue)
    {
        currentEngineThrottle = currentSliderValue;
    }


    private void FixedUpdate()
    {
        currentBrakeForce = 0f;

        if (isbrakeEnabled && currentEngineThrottle < 0.05)
        {
            ApplyBrake();
        }
    }

    private void ApplyBrake()
    {
        Vector3 velocity = flightRigidbody.linearVelocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            flightRigidbody.AddForce(
                -velocity.normalized * brakeForce,
                ForceMode.Force
            );

            currentBrakeForce = brakeForce;
        }
    }

    public void SetIsBrakeEnabled(bool target)
    {
        this.isbrakeEnabled = target;
    }

    public float GetCurrentBrakeForce()
    {
        return this.currentBrakeForce;
    }
}
