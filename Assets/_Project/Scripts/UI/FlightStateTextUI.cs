using TMPro;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class FlightStateTextUI : MonoBehaviour
{
    [SerializeField] private FlightStateController flightStateController;
    [SerializeField] private TextMeshProUGUI statePrinterUI;

    private void OnEnable()
    {
        flightStateController.StateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        flightStateController.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged(IState currentState)
    {
        statePrinterUI.text = $"현재 상태: {currentState.GetStateName()}";
    }
}
