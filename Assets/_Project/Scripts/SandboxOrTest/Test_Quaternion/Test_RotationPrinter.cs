using TMPro;
using UnityEngine;

public class Test_RotationPrinter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_1;
    [SerializeField] private TextMeshProUGUI text_0_96;
    [SerializeField] private TextMeshProUGUI text_0_5;
    [SerializeField] private TextMeshProUGUI text_0;

    [SerializeField] private Transform rotate_1;
    [SerializeField] private Transform rotate_0_96;
    [SerializeField] private Transform rotate_0_5;
    [SerializeField] private Transform rotate_0;

    void Update()
    {
        text_1.text = $"Rotation X: {FormatAngle(GetInspectorAngle(rotate_1.eulerAngles.x))}°\nRotation Z: {FormatAngle(GetInspectorAngle(rotate_1.eulerAngles.z))}°";
        text_0_96.text = $"Rotation X: {FormatAngle(GetInspectorAngle(rotate_0_96.eulerAngles.x))}°\nRotation Z: {FormatAngle(GetInspectorAngle(rotate_0_96.eulerAngles.z))}°";
        text_0_5.text = $"Rotation X: {FormatAngle(GetInspectorAngle(rotate_0_5.eulerAngles.x))}°\nRotation Z: {FormatAngle(GetInspectorAngle(rotate_0_5.eulerAngles.z))}°";
        text_0.text = $"Rotation X: {FormatAngle(GetInspectorAngle(rotate_0.eulerAngles.x))}°\nRotation Z: {FormatAngle(GetInspectorAngle(rotate_0.eulerAngles.z))}°";
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
