using UnityEngine;

public class Test_LandingDebugger : MonoBehaviour
{
    public WheelCollider[] wheels;
    public Rigidbody rb;


    void FixedUpdate()
    {

        

        Debug.Log(
    $"v={rb.linearVelocity.magnitude:F4} " +
    $"sleep={rb.IsSleeping()}"
);

    }
}
