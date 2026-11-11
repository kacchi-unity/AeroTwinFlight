using UnityEngine;

public class CalibratingState : IState
{
    private IState lastState;
    private TaxiingController taxiingController;
    private EngineController engineController;
    private FlyingController flyingController;

    public CalibratingState(
        EngineController flightEngineController,
        TaxiingController taxiingController,
        FlyingController flyingController
        )
    {
        this.engineController = flightEngineController;
        this.taxiingController = taxiingController;
        this.flyingController = flyingController;
    }

    public void Enter()
    {
        Debug.Log("Calibrating State 진입");

        engineController.StartCalibration();

        switch (lastState)
        {
            case TaxiingState:
                taxiingController.StartCalibration();
                break;

            case FlyingState:
                flyingController.StartCalibration();
                break;

            default:
                break;
        }
    }

    public void Update() { }

    public void FixedUpdate() { }

    public void Exit()
    {
        switch (lastState)
        {
            case FlyingState:
                flyingController.FinishCalibration();
                break;

            default:
                break;
        }

        engineController.FinishCalibration();
        Debug.Log("Calibrating State 종료");
    }

    public string GetStateName()
    {
        return GetType().Name;
    }

    public void SetLastState(IState targetState)
    {
        lastState = targetState;
    }
}