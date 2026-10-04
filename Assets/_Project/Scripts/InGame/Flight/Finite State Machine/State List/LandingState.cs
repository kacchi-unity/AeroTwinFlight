using UnityEngine;

public class LandingState : IState
{
    private FlightStateController flightStateController;
    private TaxiingController taxiingController;
    private Rigidbody targetRigidbody;

    private float taxiingTransitionSpeed;

    public LandingState(
        FlightStateController controller,
        TaxiingController taxiingController,
        Rigidbody targetRigidbody,
        float taxiingTransitionSpeed
        )
    {
        this.flightStateController = controller;
        this.taxiingController = taxiingController;
        this.taxiingTransitionSpeed = taxiingTransitionSpeed;
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
        if (targetRigidbody.linearVelocity.sqrMagnitude <= Mathf.Pow(taxiingTransitionSpeed,2))
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
