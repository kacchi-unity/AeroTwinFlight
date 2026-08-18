using UnityEngine;

public class CameraFollowFlightManager : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform targetFlight;
    [SerializeField] private Transform targetCamera;

    [Header("Roll 카메라 회전 고정 (z축)")]
    [SerializeField] private bool useRollFollowLock = true;

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
        
        //World Position -> Local Position
        positionOffset = targetFlight.InverseTransformPoint(targetCamera.position);

        rotationOffset = Quaternion.Inverse(targetFlight.rotation) * targetCamera.rotation;
    }

    void LateUpdate()
    {
        // 1. 위치 동기화
        targetCamera.position = targetFlight.TransformPoint(positionOffset);

        // 2. 회전 적용 (온/오프 분기)
        if (useRollFollowLock)
        {
            // [ON] Roll을 무시하고 Yaw(좌우)와 Pitch(상하) 회전만 반영
            // 비행기의 전방(Forward) 벡터를 가져옵니다.
            Vector3 flightForward = targetFlight.forward;

            // 만약 비행기가 수직으로 완전히 꽂히는 예외 상황(하늘/땅 수직 조준)이 아니라면
            if (flightForward.sqrMagnitude > 0.001f)
            {
                // 월드의 위쪽(Vector3.up)을 기준으로 삼아, Roll 성분이 완전히 제거된 회전을 생성합니다.
                Quaternion flightRotWithoutRoll = Quaternion.LookRotation(flightForward, Vector3.up);
                
                // 기존에 설정했던 오프셋 회전을 합성합니다.
                targetCamera.rotation = flightRotWithoutRoll * rotationOffset;
            }
        }
        else
        {
            // [OFF] 비행기의 모든 회전(Roll 포함)을 그대로 따라가기
            targetCamera.rotation = targetFlight.rotation * rotationOffset;
        }
    }
}
