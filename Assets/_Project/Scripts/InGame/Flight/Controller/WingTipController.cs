using UnityEngine;

public class WingTipController : MonoBehaviour
{
    [SerializeField] private bool activeWingTip = true;
    [SerializeField] private TrailRenderer leftWingTip;
    [SerializeField] private TrailRenderer rightWingTip;
    [SerializeField] private FlightStateController flightStateController;



    private void OnEnable()
    {
        if (flightStateController != null)
        {
            flightStateController.StateChanged += SetWingTip;
        }
    }

    private void OnDisable()
    {
        if (flightStateController != null)
        {
            flightStateController.StateChanged -= SetWingTip;
        }
    }

    private void SetWingTip(IState currentState)
    {
        if (!activeWingTip)
        {
            return;
        }

        if (leftWingTip != null && rightWingTip != null)
        {
            leftWingTip.enabled = (currentState is FlyingState);
            rightWingTip.enabled = (currentState is FlyingState);
        }
    }
}
