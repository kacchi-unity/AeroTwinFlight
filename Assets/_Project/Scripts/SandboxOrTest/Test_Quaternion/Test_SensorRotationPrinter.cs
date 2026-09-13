using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Test_SensorRotationPrinter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private WheelCollider LeftWheel;
    [SerializeField] private WheelCollider BackWheel;
    [SerializeField] private Rigidbody rb;

    private void OnEnable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated += ProcessPrintSensorRotation;
    }

    private void OnDisable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated -= ProcessPrintSensorRotation;
    }

    void ProcessPrintSensorRotation(Quaternion targetQuaternion)
    {
        Vector3 currentRot = targetQuaternion.eulerAngles;

        float pitch = GetInspectorAngle(currentRot.x); // Rotation X
        float yaw = GetInspectorAngle(currentRot.y);   // Rotation Y
        float roll = GetInspectorAngle(currentRot.z);  // Rotation Z

        text.text =
            $"[ABS Quaternion]\n" +
            $"  Pitch : {FormatAngle(pitch),6:F1}°\n" +
            $"  Yaw   : {FormatAngle(yaw),6:F1}°\n" +
            $"  Roll  : {FormatAngle(roll),6:F1}°";

        text2.text =
            $"[Taxiing]\n" +
            $"  Speed       : {rb.linearVelocity.magnitude,6:F1} m/s\n" +
            $"  Motor Torque: {LeftWheel.motorTorque,6:F0} Nm\n" +
            $"  Brake Torque: {LeftWheel.brakeTorque,6:F0} Nm\n" +
            $"  Steering    : {BackWheel.steerAngle,6:F1}°";

    }

    private float GetInspectorAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private string FormatAngle(float angle)
    {
        //if (Mathf.Abs(angle) < 0.05f) angle = 0f;

        return angle.ToString(" 0.0;-0.0; 0.0");
    }

    
}
