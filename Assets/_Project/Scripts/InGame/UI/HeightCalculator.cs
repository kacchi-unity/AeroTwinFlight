using System;
using UnityEngine;

public class HeightCalculator : MonoBehaviour
{
    [SerializeField] private Transform flightTransform;
    [SerializeField] private WheelCollider wheelCollider;
    [SerializeField] private GeoMapGenerator geoMapGenerator;
    [SerializeField] private float groundHeight = 5f;
    [SerializeField] private float respawnHeight = -40f;

    private float currentHeight = 0f;
    private float heightOffset = 0f;
    private float lastHeight = 0f;
    private bool isFirstGrounded;
    private bool isFirstWaypointChanged;
    private bool isBaseStation;

    public event Action OnRespawnHeightExceeded;

    private void OnEnable()
    {
        geoMapGenerator.ChangedWaypoint += SetHeightOffset;
    }

    private void OnDisable()
    {
        geoMapGenerator.ChangedWaypoint -= SetHeightOffset;
    }

    private void Start()
    {
        isFirstGrounded = false;
        isBaseStation = true;
        currentHeight = 0f;
    }

    private void Update()
    {
        if (!isFirstGrounded)
        {
            CheckFirstOffset();
            return;
        }

        if (!isFirstWaypointChanged)
        {
            currentHeight = flightTransform.position.y - heightOffset;
        }

        else
        {
            currentHeight = heightOffset + (flightTransform.position.y - lastHeight);
        }

        if (currentHeight <= respawnHeight && isBaseStation)
        {
            OnRespawnHeightExceeded?.Invoke();
        }

    }

    private void CheckFirstOffset()
    {
        if (wheelCollider!= null && wheelCollider.isGrounded)
        {
            isFirstGrounded = true;

            heightOffset = flightTransform.position.y;
        }
    }

    private void SetHeightOffset(WaypointData currentWaypoint)
    {
        isFirstWaypointChanged = true;

        isBaseStation = currentWaypoint.IsBaseStation;

        float pointsElevation = isBaseStation ? - groundHeight : (float)currentWaypoint.Height;

        float loadHeightOffset = geoMapGenerator.GetHeightOffset();

        this.heightOffset = loadHeightOffset + pointsElevation;

        lastHeight = flightTransform.position.y;
    }

    public float GetCurrentHeight()
    {
        return this.currentHeight;
    }
}
