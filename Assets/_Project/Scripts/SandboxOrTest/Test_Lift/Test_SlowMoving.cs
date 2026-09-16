using TMPro;
using UnityEngine;

public class Test_SlowMoving : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    public WheelCollider[] wheels;
    [SerializeField] private bool isThrustOn;
    public Rigidbody rb;
    float thrust;


    void Update()
    {

        tmp.text =
            $"[Case: Wheel Torque = 10^(-4) Nm]\n\n" +
            $"Speed: {rb.linearVelocity.magnitude:F1} m/s\n" +
            $"Sleeping: {rb.IsSleeping():F1}\n" +
            $"Thrust: {thrust} N";
        

    }

    void FixedUpdate()
    {
        foreach (var wc in wheels)
        {
            wc.motorTorque = 0.0001f;
        }

        thrust = isThrustOn ? 1000 : 0;

        rb.AddForce(transform.forward * thrust);
    }
}