using UnityEngine;

public class FlyingState : IState
{
    private FlightStateController flightStateController;
    private FlyingController flyingController;
    private BrakeController brakeController;
    private GeoMapGenerator geoMapGenerator;

    public FlyingState(
        FlightStateController controller,
        FlyingController flyingController,
        BrakeController brakeController,
        GeoMapGenerator geoMapGenerator
        )
    {
        this.flightStateController = controller;
        this.flyingController = flyingController;
        this.brakeController = brakeController;
        this.geoMapGenerator = geoMapGenerator;
    }

    public void Enter()
    {
        Debug.Log("Flying State 진입");

        flyingController.InitializeRotationReferecne();

        flightStateController.BothTriggerGrounded += OnBothTriggerGrounded;

        brakeController.SetIsBrakeEnabled(false);

        geoMapGenerator.SetIsWaypointChangeAllowed(true);
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

        flightStateController.BothTriggerGrounded -= OnBothTriggerGrounded;

        brakeController.SetIsBrakeEnabled(true);

        geoMapGenerator.SetIsWaypointChangeAllowed(false);
    }

    public string GetStateName()
    {
        return GetType().Name;
    }

    private void OnBothTriggerGrounded()
    {
        flightStateController.ChangeState(flightStateController.LandingState);
    }
}
