using UnityEngine;

public class TakeoffState : IState
{
    private FlightStateController flightStateController;
    private TakeoffController takeoffController;
    private Rigidbody targetRigidbody;
    private float takeoffToTaxiingSpeed;
    private TakeoffGuideGUI takeoffGuideGUI;


    public TakeoffState(
        FlightStateController controller,
        TakeoffController takeoffController,
        Rigidbody targetRigidbody,
        float takeoffToTaxiingSpeed,
        TakeoffGuideGUI takeoffGuideGUI
        )
    {
        this.flightStateController = controller;
        this.takeoffController = takeoffController;
        this.targetRigidbody = targetRigidbody;
        this.takeoffToTaxiingSpeed = takeoffToTaxiingSpeed;
        this.takeoffGuideGUI = takeoffGuideGUI;
    }

    public void Enter()
    {
        Debug.Log("Takeoff State 진입");

        takeoffController.InitializeOffset();

        flightStateController.BothTriggerAirborne += OnBothTriggerAirborne;

        takeoffGuideGUI.OnBlinkGUI();
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        float currentSqrSpeed = targetRigidbody.linearVelocity.sqrMagnitude;

        if (currentSqrSpeed < Mathf.Pow(takeoffToTaxiingSpeed, 2f))
        {
            flightStateController.ChangeState(flightStateController.TaxiingState);
        }

        takeoffController.UpdateRotation();

    }

    public void Exit()
    {
        Debug.Log("Takeoff State 종료");

        flightStateController.BothTriggerAirborne -= OnBothTriggerAirborne;

        takeoffGuideGUI.OffBlinkGUI();
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
