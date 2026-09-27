using UnityEngine;

public class CalibratingState : IState
{
    private FlightStateController controller;
    private TaxiingController taxiingController;
    private EngineController engineController;

    private string stateName = "Calibrating";

    public CalibratingState(
        FlightStateController controller,
        EngineController flightEngineController,
        TaxiingController taxiingController
        )
    {
        this.controller = controller;
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
        return stateName;
    }
}