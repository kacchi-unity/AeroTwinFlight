using UnityEngine;

public class CameraFollowFlightManager : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform targetFlight;
    [SerializeField] private Transform targetCamera;

    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    void Start()
    {
        if (targetFlight == null || targetCamera == null)
        {
            Debug.LogError("오브젝트 연결을 확인하세요");
            enabled = false;
            return;
        }

        positionOffset = targetFlight.InverseTransformPoint(targetCamera.position);

        rotationOffset = Quaternion.Inverse(targetFlight.rotation) * targetCamera.rotation;
    }

    void LateUpdate()
    {
        targetCamera.position = targetFlight.TransformPoint(positionOffset);

        targetCamera.rotation = targetFlight.rotation * rotationOffset;
    }
}
