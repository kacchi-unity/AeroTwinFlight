using UnityEngine;

public class FlyingState : IState
{
    private FlightStateController flightStateController;
    private FlyingController flyingController;
    private WheelCollider[] wheelColliders;

    private string stateName = "Flying";

    public FlyingState(
        FlightStateController controller,
        FlyingController flyingController,
        WheelCollider[] wheelColliders
        )
    {
        this.flightStateController = controller;
        this.flyingController = flyingController;
        this.wheelColliders = wheelColliders;
    }

    public void Enter()
    {
        Debug.Log("Flying State 진입");
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {

        flyingController.UpdateRotation();
    }

    public void Exit()
    {
        Debug.Log("Flying State 종료");
    }

    public string GetStateName()
    {
        return stateName;
    }
}
