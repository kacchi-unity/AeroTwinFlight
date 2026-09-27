using System;
using UnityEngine;

public class EngineController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private float maxEnginePower = 15013.5f;
    [Range(0, 1)][SerializeField] private float slider = 0f;

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
            lastSlider = slider;
            onAllowedInputEngine = false;
        }
    }

    private void RevertSlider()
    {
        slider = lastSlider;
        onAllowedInputEngine = true;
    }

    private void FixedUpdate()
    {
        float engineRatio;

        if (onAllowedInputEngine)
        {
            engineRatio = this.slider;
        }

        else
        {
            engineRatio = 0f;
        }

        flightRigidbody.linearDamping = (engineRatio == 0) ? 1 : 0;

        currentEnginePower = engineRatio * maxEnginePower;

        flightRigidbody.AddRelativeForce(Vector3.forward * currentEnginePower, ForceMode.Force);
    }
}
