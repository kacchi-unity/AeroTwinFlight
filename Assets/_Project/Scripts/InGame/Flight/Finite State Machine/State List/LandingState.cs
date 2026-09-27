using UnityEngine;

public class LandingState : IState
{
    private FlightStateController flightStateController;
    private WheelCollider[] wheelColliders;

    private string stateName = "Landing";

    public LandingState(
        FlightStateController controller
        )
    {
        this.flightStateController = controller;
        
    }

    public void Enter()
    {
        Debug.Log("Landing State 진입");
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        


    }

    public void Exit()
    {
        Debug.Log("Landing State 종료");
    }

    public string GetStateName()
    {
        return stateName;
    }
}
