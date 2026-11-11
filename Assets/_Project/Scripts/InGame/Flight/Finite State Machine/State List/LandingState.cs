using UnityEngine;

public class LandingState : IState
{
    private FlightStateController flightStateController;
    private Rigidbody targetRigidbody;

    private float landingToTaxiingSpeed;

    public LandingState(
        FlightStateController controller,
        Rigidbody targetRigidbody,
        float landingToTaxiingSpeed
        )
    {
        this.flightStateController = controller;
        this.landingToTaxiingSpeed = landingToTaxiingSpeed;
        this.targetRigidbody = targetRigidbody;

    }

    public void Enter()
    {
        Debug.Log("Landing State 진입");

        flightStateController.BothTriggerAirborne += OnBothTriggerAirborne;
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        //flyingController.UpdateRotation();
        if (targetRigidbody.linearVelocity.sqrMagnitude <= Mathf.Pow(landingToTaxiingSpeed, 2))
        {
            flightStateController.ChangeState(flightStateController.TaxiingState);
        }
    }

    public void Exit()
    {
        Debug.Log("Landing State 종료");

        flightStateController.BothTriggerAirborne -= OnBothTriggerAirborne;
    }

    public string GetStateName()
    {
        return GetType().Name;
    }

    private void OnBothTriggerAirborne()
    {
        flightStateController.ChangeState(flightStateController.FlyingState);
    }
}
