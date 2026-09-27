using UnityEngine;

public class FlightLiftController : MonoBehaviour
{
    [Header("Inspector 연결")]
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private MonoBehaviour clProviderComponent;
    [Header("Lift 계수")]
    [SerializeField] private float rho = 1.225f;
    [SerializeField] private float wingArea = 16f;

    private float lift;
    private float sqrSpeed;
    private float speed;
    private Vector3 velocity;

    private ICLProvider clProvider;

    void Awake()
    {
        lift = 0f;

        clProvider = clProviderComponent as ICLProvider;

        if (clProvider == null)
        {
            Debug.LogError("ICLProvider를 구현한 컴포넌트가 필요합니다.", this);
            enabled = false;
        }

    }

    void FixedUpdate()
    {
        velocity = flightRigidbody.linearVelocity;

        sqrSpeed = velocity.sqrMagnitude;

        speed = Mathf.Sqrt(sqrSpeed);

        //Lift
        lift = 0.5f * rho * wingArea * clProvider.GetCL() * sqrSpeed;

        flightRigidbody.AddRelativeForce(Vector3.up * lift, ForceMode.Force);
    }
}
