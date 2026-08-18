using UnityEngine;
using UnityEngine.UIElements;

public class Test_PositionController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private Vector3 initialPosition;

    private void OnEnable()
    {
        CalibrateManager.onStartCalibrate += ResetTargetPosition;
    }

    private void OnDisable()
    {
        CalibrateManager.onStartCalibrate -= ResetTargetPosition;
    }

    private void Start()
    {
        initialPosition = targetTransform.position;
    }

    void ResetTargetPosition()
    {
        targetTransform.position = initialPosition;
    }
}
