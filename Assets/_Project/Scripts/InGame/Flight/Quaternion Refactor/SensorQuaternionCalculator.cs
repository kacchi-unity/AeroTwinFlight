using System;
using UnityEngine;

public class SensorQuaternionCalculator : MonoBehaviour
{
    [Header("Absolute 모드 제어 설정")]
    [Header("상보 필터(Complementary Filter) 자이로 계수 [0 ~ 1]")]
    [SerializeField, Range(0f, 1f)] private float gyroWeight = 0.96f;

    [Header("회전 감도 설정 (1 권장)")]
    [SerializeField] private float rotationSpeed = 1f;

    [Header("Attitude Control 모드 제어 설정")]
    [Tooltip("미세 손떨림 방지 데드존 각도(°)")]
    [SerializeField] private float deadzoneAngle = 4f;

    [Tooltip("센서 손목 기울기 최대 한계 각도 (°, 100% 최대 파워 출력 호출 기준 각도)")]
    [SerializeField] private float maxInputTiltAngle = 45f;

    [Header("오브젝트 반응성 설정")]
    [Tooltip("비행기 기체가 최대로 기울어질 수 있는 시각적 한계 각도 (도)")]
    [SerializeField] private float maxVisibleTurnAngle = 35f;

    [Tooltip("센서 기울임(Roll) 대비 좌우 방향 선회 초당 회전 속도 (°/s)")]
    [SerializeField] private float yawTurnSpeed = 45f;

    private Quaternion absoluteQuaternion = Quaternion.identity;
    private Quaternion attitudeControlQuaternion = Quaternion.identity;

    private Quaternion initialRotation;
    bool isCalculateAllowed = false;

    private MPU6050Data latestSensorData;
    private bool hasSensorData = false;

    private float initialYawRotation = 0f;
    private float virtualCurrentYaw_AC = 0f;

    public static event Action<Quaternion> OnAbsoluteQuaternionCalculated;
    public static event Action<Quaternion> OnAttitudeControlQuaternionCalculated;

    private void OnEnable()
    {
        MPU6050DataConverter.OnDataReady += UpdateLatestSensorData;
        CalibrateManager.onStartCalibrate += StartCalibrate;
        CalibrateManager.onFinishCalibrate += FinishCalibrate;
    }

    private void OnDisable()
    {
        MPU6050DataConverter.OnDataReady -= UpdateLatestSensorData;
        CalibrateManager.onStartCalibrate -= StartCalibrate;
        CalibrateManager.onFinishCalibrate -= FinishCalibrate;
    }

    void Start()
    {
        InitializeValue();
    }

    /// <summary>
    /// 스크립트 내 저장된 MPU6050 센서 데이터와 상보 필터를 이용해 Absolute, Attitude Control Quaternion을 계산하고 저장
    /// </summary>
    void Update()
    {
        if (!isCalculateAllowed || !hasSensorData)
        {
            return;
        }

        this.absoluteQuaternion = CalculateAbsoluteQuaternion(this.latestSensorData);

        this.attitudeControlQuaternion = CalculateAttitudeControlQuaternion(this.absoluteQuaternion);

        OnAbsoluteQuaternionCalculated?.Invoke(absoluteQuaternion);

        OnAttitudeControlQuaternionCalculated?.Invoke(attitudeControlQuaternion);
    }

    void StartCalibrate()
    {
        isCalculateAllowed = false;
        hasSensorData = false;
        Debug.Log("보정이 시작되어 쿼터니안 계산을 중단합니다.");
    }

    void FinishCalibrate()
    {
        ResetValue();

        isCalculateAllowed = true;
        Debug.Log("보정이 끝나 쿼터니안 계산을 재개합니다.");
    }

    void InitializeValue()
    {
        //Process Absolute Mode Setting
        initialRotation = transform.rotation;
        absoluteQuaternion = initialRotation;

        //Process Attitude Control Mode Setting
        initialYawRotation = transform.eulerAngles.y;
        virtualCurrentYaw_AC = initialYawRotation;
    }

    void ResetValue()
    {
        //Process Absolute Mode Reset
        absoluteQuaternion = initialRotation;

        //Process Attitude Control Reset
        virtualCurrentYaw_AC = transform.eulerAngles.y;
    }

    /// <summary>
    /// MPU6050 센서 데이터를 이벤트 호출 마다 저장
    /// </summary>
    private void UpdateLatestSensorData(MPU6050Data sensorData)
    {
        latestSensorData = sensorData;
        hasSensorData = true;
    }

    private Quaternion CalculateAbsoluteQuaternion(MPU6050Data sensorData)
    {
        float deltaTime = Time.deltaTime;

        //축 매핑: Real MPU6050 Sensor -> Unity
        Vector3 gyroMapped = new Vector3(-sensorData.gyroX, -sensorData.gyroZ, -sensorData.gyroY);
        Vector3 accelMapped = new Vector3(sensorData.accelX, sensorData.accelZ, sensorData.accelY);

        //자이로 처리 (로컬 회전 누적)
        // currentRotation이 월드 기준이므로, gyroPredictedRotation도 월드기준 결과물 쿼터니안
        Vector3 gyroDeltaEuler = deltaTime * rotationSpeed * gyroMapped;

        //test
        //gyroDeltaEuler.y = 0f;

        Quaternion gyroDeltaRotation = Quaternion.Euler(gyroDeltaEuler.x, gyroDeltaEuler.y, gyroDeltaEuler.z);
        // 자이로 데이터만으로 예측한 현재 프레임의 회전값
        Quaternion gyroPredictedRotation = absoluteQuaternion * gyroDeltaRotation;

        //가속도 처리 (현재 자세에서 중력 방향 예측)
        //accelDirection은 오직 센서 자기 자신의 몸통을 기준으로,
        //하늘이 어느 쪽인지만 나타내는 로컬 좌표계의 벡터
        Vector3 accelDirection = accelMapped.normalized;
        //가속도 센서가 측정하는 월드 중력 반대 방향 벡터를 세계 좌표계로 변환
        //공식: Vector' = Quaternion * Vector (벡터를 쿼터니안만큼 회전)
        //skyInWorldByGyro = 자이로 센서 눈으로 바라본 세계 좌표계의 가속도 방향
        Vector3 skyInWorldByGyro = gyroPredictedRotation * accelDirection;

        //실제 중력에 반대인 하늘 방향(Vector3.up)과의 오차 회전 구하기 (이 회전은 Roll/Pitch 오차만 가짐)
        Quaternion accelCorrectionRotation = Quaternion.FromToRotation(skyInWorldByGyro, Vector3.up);

        //상보 필터 결합
        //오차 회전(accCorrection)을 아주 미세한 비율(1 - gyroWeight)만큼만 취함
        Quaternion filteredAccelCorrection = Quaternion.Lerp(Quaternion.identity, accelCorrectionRotation, 1f - gyroWeight);

        //세계 좌표계 오차이므로 왼쪽에 곱해줌 (Yaw는 건드리지 않고 Roll/Pitch만 보정됨)
        //gyroPredictedRotation는 자이로가 예측한 비행기 자세, filteredAccelCorrection는 가속도 보정 회전
        //쿼터니안 곱 = gyroPredictedRotation 먼저 회전 후 그 다음에 filteredAccelCorrection 회전 (오른쪽에서 왼쪽으로 순차 적용)
        absoluteQuaternion = filteredAccelCorrection * gyroPredictedRotation;

        //정규화 및 적용
        absoluteQuaternion = Quaternion.Normalize(absoluteQuaternion);

        return absoluteQuaternion;
    }

    private Quaternion CalculateAttitudeControlQuaternion(Quaternion absoluteRotation)
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
        virtualCurrentYaw_AC += rollPower * yawTurnSpeed * Time.deltaTime;

        // 짐벌락 방지용 쿼터니안 조합 (Quaternion.Euler 대신 AngleAxis 곱셈 활용)
        Quaternion yawRot = Quaternion.AngleAxis(virtualCurrentYaw_AC, Vector3.up);
        Quaternion pitchRot = Quaternion.AngleAxis(targetPitch, Vector3.right);
        Quaternion rollRot = Quaternion.AngleAxis(targetRoll, Vector3.forward);

        // 쿼터니안 합성
        Quaternion totalRotation = yawRot * pitchRot * rollRot;

        return totalRotation;
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
