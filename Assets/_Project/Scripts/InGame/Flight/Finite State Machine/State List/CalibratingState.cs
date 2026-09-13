using UnityEngine;

public class CalibratingState : IState
{
    private FlightStateController controller;
    private StateMachine stateMachine;
    private FlightTaxiController flightTaxiController;

    public CalibratingState(
        FlightStateController controller,
        StateMachine stateMachine,
        FlightTaxiController flightTaxiController)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
        this.flightTaxiController = flightTaxiController;
    }

    public void Enter()
    {
        Debug.Log("Calibrating State 진입");
        flightTaxiController.StartCalibration();
    }
    public void Update() { }
    public void FixedUpdate() { }
    public void Exit()
    {
        Debug.Log("Calibrating State 종료");
    }
}