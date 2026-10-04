using UnityEngine;

public class TestCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform targetFlight;

    private Vector3 initialLocalOffset;
    private Quaternion initialLocalRotation;

    private void Start()
    {
        // 씬에 배치해둔 카메라의 초기 상태를
        // 비행기 기준으로 저장
        initialLocalOffset =
            targetFlight.InverseTransformPoint(transform.position);

        initialLocalRotation =
            Quaternion.Inverse(targetFlight.rotation) *
            transform.rotation;
    }

    private void LateUpdate()
    {
        // =========================
        // 1. 비행기의 Yaw만 추출
        // =========================

        Vector3 flatForward = targetFlight.forward;
        flatForward.y = 0f;

        if (flatForward.sqrMagnitude < 0.001f)
            return;

        flatForward.Normalize();

        Quaternion yawRotation =
            Quaternion.LookRotation(flatForward, Vector3.up);


        // =========================
        // 2. 위치
        // =========================

        transform.position =
            targetFlight.position +
            yawRotation * initialLocalOffset;


        // =========================
        // 3. 회전
        // =========================

        transform.rotation =
            yawRotation * initialLocalRotation;
    }
}