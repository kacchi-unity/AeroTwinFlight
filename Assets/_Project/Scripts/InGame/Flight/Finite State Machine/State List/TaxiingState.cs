using UnityEngine;

public class TaxiingState : IState
{
    private FlightStateController flightStateController;
    private TaxiingController taxiingController;
    private Rigidbody targetRigidbody;
    private float taxiingToTakeoffSpeed;

    public TaxiingState(
        FlightStateController flightStateController,
        TaxiingController taxiingController,
        Rigidbody targetRigidbody,
        float taxiingToTakeoffSpeed
        )
    {
        this.flightStateController = flightStateController;
        this.taxiingController = taxiingController;
        this.targetRigidbody = targetRigidbody;
        this.taxiingToTakeoffSpeed = taxiingToTakeoffSpeed;
        
    }

    public void Enter()
    {
        Debug.Log("Taxiing State 진입");

        taxiingController.ResetSteerAngle();
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {
        float currentSqrSpeed = targetRigidbody.linearVelocity.sqrMagnitude;

        if (currentSqrSpeed >= Mathf.Pow(taxiingToTakeoffSpeed, 2f))
        {
            flightStateController.ChangeState(flightStateController.TakeoffState);
        }

        else
        {
            taxiingController.UpdateTaxiing();
        }
        
    }

    public void Exit()
    {
        Debug.Log("Taxiing State 종료");

    }

    public string GetStateName()
    {
        return GetType().Name;
    }
}
