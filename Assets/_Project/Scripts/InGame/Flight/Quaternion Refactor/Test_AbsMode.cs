using UnityEngine;

public class Test_AbsMode : MonoBehaviour
{

    private void OnEnable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated += UpdateRotation;
    }

    private void OnDisable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated -= UpdateRotation;
    }

    void UpdateRotation(Quaternion currentRotation)
    {
        transform.rotation = currentRotation;
    }
}
