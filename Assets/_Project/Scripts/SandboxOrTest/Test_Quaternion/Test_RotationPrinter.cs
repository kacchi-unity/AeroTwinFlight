using TMPro;
using UnityEngine;

public class Test_RotationPrinter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Transform rotate;

    void Update()
    {
        Vector3 currentRot = rotate.localEulerAngles;

        text.text = $"Rotation X: {FormatAngle(GetInspectorAngle(currentRot.x))}°";
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
