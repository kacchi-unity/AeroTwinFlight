using UnityEngine;

public class TaxiingState : IState
{
    private FlightStateController controller;
    private StateMachine stateMachine;
    private FlightTaxiController flightTaxiController;

    public TaxiingState(
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
        Debug.Log("Taxiing State 진입");
    }

    public void Update() { }

    public void FixedUpdate()
    {
        flightTaxiController.UpdateTaxiing();
    }

    public void Exit()
    {
        Debug.Log("Taxiing State 종료");
    }
}
