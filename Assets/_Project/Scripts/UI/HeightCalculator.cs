using UnityEngine;

public class HeightCalculator : MonoBehaviour
{
    [SerializeField] private Transform flightTransform;
    [SerializeField] private WheelCollider wheelCollider;
    [SerializeField] private GeoMapGenerator geoMapGenerator;
    [SerializeField] private float groudHeight = 2.8f;

    private float currentHeight = 0f;
    private float heightOffset = 0f;
    private float lastHeight = 0f;
    private bool isFirstGrounded;
    private bool isFirstWaypointChanged;

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
        currentHeight = -1000f;
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

    }

    private void CheckFirstOffset()
    {
        if (wheelCollider!= null && wheelCollider.isGrounded)
        {
            isFirstGrounded = true;
            Debug.Log("True True True");

            heightOffset = flightTransform.position.y;
        }
    }

    private void SetHeightOffset(WaypointData currentWaypoint)
    {
        isFirstWaypointChanged = true;

        bool isBaseStation = currentWaypoint.IsBaseStation;

        float pointsElevation = isBaseStation ? groudHeight : (float)currentWaypoint.Height;

        float loadHeightOffset = geoMapGenerator.GetHeightOffset();

        this.heightOffset = loadHeightOffset - pointsElevation;

        lastHeight = flightTransform.position.y;
    }

    public float GetCurrentHeight()
    {
        return this.currentHeight;
    }
}
