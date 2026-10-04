using UnityEngine;
using UnityEngine.UI;

public class BrakeController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private float brakeForce = 3000f;

    //test
    [SerializeField] private Slider engineSlider;

    private bool isbrakeEnabled = true;
    private float currentBrakeForce = 0; //For UI Print

    private void FixedUpdate()
    {
        currentBrakeForce = 0f;

        if (isbrakeEnabled && engineSlider.value == 0)
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
