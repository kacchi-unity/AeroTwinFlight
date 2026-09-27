using UnityEngine;

public class FlightDragController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;

    [Header("Drag 저항 계수")]
    [SerializeField] private float dragCoefficient = 18f;

    private float sqrSpeed;
    private Vector3 velocity;

    void FixedUpdate()
    {
        velocity = flightRigidbody.linearVelocity;

        sqrSpeed = velocity.sqrMagnitude;

        //Drag
        if (sqrSpeed > 0.01f)
        {
            float drag = dragCoefficient * sqrSpeed;

            flightRigidbody.AddForce(velocity.normalized * drag * (-1), ForceMode.Force);
        }
    }
}
