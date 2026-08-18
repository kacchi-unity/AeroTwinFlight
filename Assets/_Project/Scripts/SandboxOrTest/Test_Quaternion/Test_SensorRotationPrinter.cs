using TMPro;
using UnityEngine;

public class Test_SensorRotationPrinter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        FlightRotationController.OnQuaternionCalculateFinish += ProcessPrintSensorRotation;
    }

    private void OnDisable()
    {
        FlightRotationController.OnQuaternionCalculateFinish -= ProcessPrintSensorRotation;
    }

    void ProcessPrintSensorRotation(Quaternion targetQuaternion)
    {
        Vector3 currentRot = targetQuaternion.eulerAngles;

        float pitch = GetInspectorAngle(currentRot.x); // Rotation X
        float yaw = GetInspectorAngle(currentRot.y);   // Rotation Y
        float roll = GetInspectorAngle(currentRot.z);  // Rotation Z

        text.text = $"Sensor Rotation\n"+
                    $"Rotation X (Pitch): {FormatAngle(pitch)}°\n" +
                    $"Rotation Y (Yaw): {FormatAngle(yaw)}°\n" +
                    $"Rotation Z (Roll): {FormatAngle(roll)}°";
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
