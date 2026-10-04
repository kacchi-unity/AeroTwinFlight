using System;
using UnityEngine;
using UnityEngine.UI;

public class EngineController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private float maxEnginePower = 15013.5f;
    [SerializeField] private BrakeController brakeController;

    //test
    [SerializeField] private Slider engineSlider;

    private float lastSlider;//추후 삭제 후 슬라이더 센서값 도입

    private bool onAllowedInputEngine = true;

    float currentEnginePower = 0f;

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
            lastSlider = engineSlider.value;
            onAllowedInputEngine = false;
        }
    }

    private void RevertSlider()
    {
        engineSlider.value = lastSlider;
        onAllowedInputEngine = true;
    }

    private void FixedUpdate()
    {
        float engineRatio = (onAllowedInputEngine) ? this.engineSlider.value : 0f;

        currentEnginePower = engineRatio * maxEnginePower;

        flightRigidbody.AddRelativeForce(Vector3.forward * currentEnginePower, ForceMode.Force);
    
    }

    public float GetCurrentEnginePower()
    {
        return currentEnginePower;
    }
}
