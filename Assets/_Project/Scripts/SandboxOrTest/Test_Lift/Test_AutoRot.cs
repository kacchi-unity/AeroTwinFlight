using UnityEngine;

public class Test_AutoRot : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;


    [Header("Pitch")]
    [SerializeField] private float pitchAngle = 10f;
    [SerializeField] private bool isPitchUp = false;

    [Header("Roll")]
    [SerializeField] private float rollAngle = 10f;
    [SerializeField] private bool isRollUp = false;

    [Header("Yaw")]
    [SerializeField] private float yawAngle = 10f;
    [SerializeField] private bool isYawUp = false;

    private void FixedUpdate()
    {
        Vector3 currentEuler = rb.rotation.eulerAngles;

        float pitch = isPitchUp ? -pitchAngle : 0f;
        float roll = isRollUp ? rollAngle : 0f;
        float yaw = isYawUp ? yawAngle : currentEuler.y;

        Quaternion targetRotation = Quaternion.Euler(
            pitch,
            yaw,
            roll
        );

        rb.MoveRotation(targetRotation);
    }
}
