using UnityEngine;

public class EngineController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private float maxEnginePower = 15013.5f;
    [SerializeField] private BrakeController brakeController;

    private bool onAllowedInputEngine = true;

    float currentEnginePower = 0f;

    float currentEngineThrottle = 0f;

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

    public void StartCalibration()
    {
        ResetPhysics();
    }

    public void FinishCalibration()
    {
        RevertSlider();
    }

    private void ResetPhysics()
    {
        if (flightRigidbody != null)
        {
            onAllowedInputEngine = false;
        }
    }

    private void RevertSlider()
    {
        onAllowedInputEngine = true;
    }

    private void FixedUpdate()
    {
        float engineRatio = (onAllowedInputEngine) ? this.currentEngineThrottle : 0f;

        currentEnginePower = engineRatio * maxEnginePower;

        flightRigidbody.AddRelativeForce(Vector3.forward * currentEnginePower, ForceMode.Force);
    
    }

    public float GetCurrentEnginePower()
    {
        return currentEnginePower;
    }
}
