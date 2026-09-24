using TMPro;
using UnityEngine;

public class Test_Lift : MonoBehaviour
{
    public WheelCollider[] wheels;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform tr;
    [SerializeField] private TextMeshProUGUI tmp;
    [Range(0,1)][SerializeField] private float slider = 0f;
    [SerializeField] private float maxEnginePower = 15000f;
    [SerializeField] private float dragCoefficient = 18f;

    [Header ("Lift Coeff")]
    [SerializeField] private float rho = 1.225f;
    [SerializeField] private float wingArea = 16f;
    [SerializeField] private Test_Cl test_cl;

    float lift;
    float sqrSpeed;
    float speed;
    Vector3 velocity;
    float enginePower;

    void Update()
    {
        
        tmp.text = 
            $"Height: {rb.GetComponent<Transform>().position.y - 31.4f:F1}\n" +
            $"Speed: {speed:F1} m/s\n" +
            $"Engine: {enginePower:F1}\n" +
            $"Lift: {lift:F1}";

        
    }

    void Awake()
    {
        lift = 0;
        enginePower = 0;

    }

    void FixedUpdate()
    {
        foreach (var wc in wheels)
        {
            wc.motorTorque = 0.0001f;
        }

        rb.linearDamping = slider == 0 ? 1 : 0;  //brake

        velocity = rb.linearVelocity;

        sqrSpeed = velocity.sqrMagnitude;

        speed = Mathf.Sqrt(sqrSpeed);

        lift = 0.5f * rho * wingArea * test_cl.GetCL() * sqrSpeed;

        enginePower = this.slider * maxEnginePower;

        rb.AddRelativeForce(Vector3.forward * enginePower, ForceMode.Force);

        //Lift
        rb.AddRelativeForce(Vector3.up * lift, ForceMode.Force);

        //Drag
        if (sqrSpeed > 0.01f)
        {
            float drag = dragCoefficient * sqrSpeed;

            rb.AddForce(velocity.normalized * drag * (-1), ForceMode.Force);
        }
    }
}
