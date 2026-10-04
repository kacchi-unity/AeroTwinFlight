using UnityEngine;

public class TakeoffState : IState
{
    private FlightStateController flightStateController;
    private TaxiingController taxiingController;
    private TakeoffController takeoffController;
    private Rigidbody targetRigidbody;
    private float takeoffSpeed;


    public TakeoffState(
        FlightStateController controller,
        TaxiingController taxiingController,
        TakeoffController takeoffController,
        Rigidbody targetRigidbody,
        float takeoffSpeed
        )
    {
        this.flightStateController = controller;
        this.taxiingController = taxiingController;
        this.takeoffController = takeoffController;
        this.targetRigidbody = targetRigidbody;
        this.takeoffSpeed = takeoffSpeed;
    }

    public void Enter()
    {
        Debug.Log("Takeoff State 진입");

        flightStateController.BothTriggerAirborne += OnBothTriggerAirborne;
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        

        float currentSqrSpeed = targetRigidbody.linearVelocity.sqrMagnitude;

        if (currentSqrSpeed < Mathf.Pow(takeoffSpeed, 2f))
        {
            flightStateController.ChangeState(flightStateController.TaxiingState);
        }

        takeoffController.UpdateRotation();

    }

    public void Exit()
    {
        Debug.Log("Takeoff State 종료");

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
