using UnityEngine;

public class AttitudeFlightRotationController : MonoBehaviour
{
    [Header("센서 제어 설정")]
    [Tooltip("미세 손떨림 방지 데드존 각도(°)")]
    [SerializeField] private float deadzoneAngle = 4f;

    [Tooltip("100% 최대 파워 출력 호출 센서 기울기 최대 한계 각도 (°)")]
    [SerializeField] private float maxInputTiltAngle = 45f;

    [Header("오브젝트 반응성 설정")]
    [Tooltip("비행기 기체가 최대로 기울어질 수 있는 시각적 한계 각도 (도)")]
    [SerializeField] private float maxVisibleTurnAngle = 35f;

    [Tooltip("센서 기울임(Roll) 대비 좌우 방향 선회 초당 회전 속도 (°/s)")]
    [SerializeField] private float yawTurnSpeed = 45f;

    [Tooltip("센서 각도 반응 추종 속도 (높을수록 선회 빠름)")]
    [SerializeField] private float smoothSpeed = 5f;

    private float initialYaw = 0f;
    private float currentYaw = 0f;

    private void OnEnable()
    {
        FlightRotationController.OnQuaternionCalculateFinish += ProcessAttitudeRotation;
        CalibrateManager.onStartCalibrate += RestCurrentYaw;
    }

    private void OnDisable()
    {
        FlightRotationController.OnQuaternionCalculateFinish -= ProcessAttitudeRotation;
        CalibrateManager.onStartCalibrate -= RestCurrentYaw;
    }

    private void Start()
    {
        initialYaw = transform.eulerAngles.y;
        RestCurrentYaw();
    }

    void RestCurrentYaw()
    {
        currentYaw = initialYaw;
    }

    void ProcessAttitudeRotation(Quaternion absoluteRotation)
    {
        // 절대 쿼터니안에서 벡터 성분을 추출
        Vector3 forwardInWorld = absoluteRotation * Vector3.forward;
        Vector3 upInWorld = absoluteRotation * Vector3.up;
        Vector3 rightInWorld = absoluteRotation * Vector3.right;

        //역 삼각함수를 사용하여 각도 추출
        float rawPitch = Mathf.Asin(Mathf.Clamp(forwardInWorld.y, -1f, 1f)) * Mathf.Rad2Deg * (-1f);
        float rawRoll = Mathf.Atan2(rightInWorld.y, upInWorld.y) * Mathf.Rad2Deg * (-1f);

        //적용할 Power 비율 계산 (-1.0 ~ 1.0)
        float pitchPower = CalculatePowerRatio(rawPitch);
        float rollPower = CalculatePowerRatio(rawRoll);

        // 시각적 회전 각도 계산
        float targetPitch = pitchPower * maxVisibleTurnAngle;
        float targetRoll = -rollPower * maxVisibleTurnAngle; // 오른쪽 기울임 시 Z축 반대 방향 뱅킹

        // Roll이 기울어져 있는 동안만 진행 방향(Yaw) 누적
        currentYaw += rollPower * yawTurnSpeed * Time.deltaTime;

        // 짐벌락 방지용 쿼터니안 조합 (Quaternion.Euler 대신 AngleAxis 곱셈 활용)
        Quaternion yawRot = Quaternion.AngleAxis(currentYaw, Vector3.up);
        Quaternion pitchRot = Quaternion.AngleAxis(targetPitch, Vector3.right);
        Quaternion rollRot = Quaternion.AngleAxis(targetRoll, Vector3.forward);

        // 쿼터니안 합성
        Quaternion targetRotation = yawRot * pitchRot * rollRot;

        // 회전 적용
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }

    private float CalculatePowerRatio(float currentAngle)
    {
        float absAngle = Mathf.Abs(currentAngle);

        if (absAngle <= deadzoneAngle)
        {
            return 0f;
        }

        if (absAngle >= maxInputTiltAngle)
        {
            return 1f * Mathf.Sign(currentAngle);
        }

        float effectiveAngle = absAngle - deadzoneAngle;
        float maxEffectiveAngle = maxInputTiltAngle - deadzoneAngle;
        float power = Mathf.Clamp01(effectiveAngle / maxEffectiveAngle);

        // 원래 기울어진 방향(부호) 복원
        return power * Mathf.Sign(currentAngle);
    }
}
