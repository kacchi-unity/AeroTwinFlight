using UnityEngine;

public class TakeoffState : IState
{
    private FlightStateController flightStateController;
    private FlyingController flyingController;
    private Rigidbody targetRigidbody;
    private float takeoffSpeed;
    private float takeoffHeightDelta;

    private float takeoffStartHeight;

    private string stateName = "Take off";

    public TakeoffState(
        FlightStateController controller,
        FlyingController flyingController,
        Rigidbody targetRigidbody,
        float takeoffSpeed,
        float takeoffHeightDelta
        )
    {
        this.flightStateController = controller;
        this.flyingController = flyingController;
        this.targetRigidbody = targetRigidbody;
        this.takeoffSpeed = takeoffSpeed;
        this.takeoffHeightDelta = takeoffHeightDelta;
    }

    public void Enter()
    {
        takeoffStartHeight = targetRigidbody.position.y;
        Debug.Log("Takeoff State 진입");
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

        if (targetRigidbody.position.y > takeoffStartHeight + takeoffHeightDelta)
        {
            flightStateController.ChangeState(flightStateController.FlyingState);
        }
        flyingController.UpdateRotation();

    }

    public void Exit()
    {
        Debug.Log("Takeoff State 종료");
    }

    public string GetStateName()
    {
        return stateName;
    }
}
