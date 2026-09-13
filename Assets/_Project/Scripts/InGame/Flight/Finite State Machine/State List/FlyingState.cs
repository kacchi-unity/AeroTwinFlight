using UnityEngine;

public class FlyingState : IState
{
    private FlightStateController controller;
    private StateMachine stateMachine;

    public FlyingState(FlightStateController controller, StateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public void Enter() { }
    public void Update() { }
    public void FixedUpdate() { }
    public void Exit() { }
}
