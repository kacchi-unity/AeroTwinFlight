using UnityEngine;

public class FlightDragController : MonoBehaviour
{
    [SerializeField] private Rigidbody flightRigidbody;

    [Header("Drag 저항 계수")]
    [SerializeField] private float dragCoefficient = 18f;

    private float sqrSpeed;
    private Vector3 velocity;
    private float drag = 0f;

    void FixedUpdate()
    {
        velocity = flightRigidbody.linearVelocity;

        sqrSpeed = velocity.sqrMagnitude;

        //Drag
        if (sqrSpeed > 0.01f)
        {
            this.drag = dragCoefficient * sqrSpeed;

            flightRigidbody.AddForce(velocity.normalized * drag * (-1), ForceMode.Force);
        }
    }

    public float GetDrag()
    {
        return this.drag;
    }
}
