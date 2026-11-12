using System;
using UnityEngine;

public class FlightRespawnManager : MonoBehaviour
{
    [SerializeField] private HeightCalculator heightCalculator;
    [SerializeField] private Transform flightTransform;
    [SerializeField] private Transform baseStationTransform;

    public event Action<Vector3, Quaternion> OnRespawnRequred;

    private Vector3 initialPositionOffset = Vector3.zero;
    private Quaternion initialRotation = Quaternion.identity;

    private void OnEnable()
    {
        if (heightCalculator != null)
        {
            heightCalculator.OnRespawnHeightExceeded += RespawnFlight;
        }
    }

    private void OnDisable()
    {
        if (heightCalculator != null)
        {
            heightCalculator.OnRespawnHeightExceeded -= RespawnFlight;
        }
    }

    private void Start()
    { 
        if (flightTransform != null)
        {
            initialPositionOffset = flightTransform.position - baseStationTransform.position;

            this.initialRotation = flightTransform.rotation;
        }
    }

    private void RespawnFlight()
    {
        if (baseStationTransform != null)
        {
            Vector3 initialPosition = baseStationTransform.position + initialPositionOffset;

            OnRespawnRequred?.Invoke(initialPosition, this.initialRotation);
        }
        
    }
}
