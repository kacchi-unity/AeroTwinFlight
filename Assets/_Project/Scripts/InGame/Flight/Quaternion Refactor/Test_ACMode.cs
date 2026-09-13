using UnityEngine;

public class Test_ACMode : MonoBehaviour
{
    [Tooltip("센서 각도 반응 추종 속도 (높을수록 선회 빠름)")]
    [SerializeField] private float smoothSpeed = 5f;

    private void OnEnable()
    {
        SensorQuaternionCalculator.OnAttitudeControlQuaternionCalculated += UpdateRotation;
    }

    private void OnDisable()
    {
        SensorQuaternionCalculator.OnAttitudeControlQuaternionCalculated -= UpdateRotation;
    }

    void UpdateRotation(Quaternion currentRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, Time.deltaTime * smoothSpeed);
    }
}
