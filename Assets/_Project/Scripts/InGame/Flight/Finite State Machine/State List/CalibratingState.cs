using UnityEngine;

public class CalibratingState : IState
{
    private TaxiingController taxiingController;
    private EngineController engineController;

    public CalibratingState(
        EngineController flightEngineController,
        TaxiingController taxiingController
        )
    {
        this.engineController = flightEngineController;
        this.taxiingController = taxiingController;
    }

    public void Enter()
    {
        Debug.Log("Calibrating State 진입");
        engineController.StartCalibration();
        taxiingController.StartCalibration();
    }

    public void Update() { }

    public void FixedUpdate() { }

    public void Exit()
    {
        engineController.FinishCalibration();
        Debug.Log("Calibrating State 종료");
    }

    public string GetStateName()
    {
        return GetType().Name;
    }
}