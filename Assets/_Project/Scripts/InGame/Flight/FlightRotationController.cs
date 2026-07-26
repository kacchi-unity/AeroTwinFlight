using UnityEngine;

public class FlightRotationController : MonoBehaviour
{
    [Header("Rotation Speed")]
    [SerializeField] private float rotationSpeed = 1f;

    [Header("상보 필터(Complementary Filter) 자이로 계수 [0 ~ 1]")]
    [SerializeField] private float gyroWeight = 0.96f;

    private Quaternion currentRotation = Quaternion.identity;

    MPU6050Data currentData = new MPU6050Data();
    bool isRotationAllowed = true;

    private void OnEnable()
    {
        MPU6050DataConverter.OnDataReady += SaveData;
        CalibrateManager.onStartCalibrate += StartCalibrate;
        CalibrateManager.onFinishCalibrate += FinishCalibrate;
    }

    private void OnDisable()
    {
        MPU6050DataConverter.OnDataReady -= SaveData;
        CalibrateManager.onStartCalibrate -= StartCalibrate;
        CalibrateManager.onFinishCalibrate -= FinishCalibrate;
    }

    void SaveData(MPU6050Data targetData)
    {
        currentData = targetData;
    }

    void StartCalibrate()
    {
        transform.localRotation = Quaternion.identity;
        currentRotation = Quaternion.identity;
        isRotationAllowed = false;
        Debug.Log("보정이 시작되어 비행기 회전이 일시 정지됩니다.");
    }

    void FinishCalibrate()
    {
        isRotationAllowed = true;
        Debug.Log("보정이 끝나 비행기 회전이 재개됩니다.");
    }

    void Update()
    {
        if (isRotationAllowed)
        {
            float deltaTime = Time.deltaTime;

            //축 매핑: Real MPU6050 Sensor -> Unity
            Vector3 gyroMapped = new Vector3(-currentData.gyroX, -currentData.gyroZ, -currentData.gyroY);
            Vector3 accelMapped = new Vector3(currentData.accelX, currentData.accelZ, currentData.accelY);

            //자이로 처리 (로컬 회전 누적)
            // currentRotation이 월드 기준이므로, gyroPredictedRotation도 월드기준 결과물 쿼터니안
            Vector3 gyroDeltaEuler = deltaTime * rotationSpeed * gyroMapped;

            //test
            //gyroDeltaEuler.y = 0f;

            Quaternion gyroDeltaRotation = Quaternion.Euler(gyroDeltaEuler.x, gyroDeltaEuler.y, gyroDeltaEuler.z);
            // 자이로 데이터만으로 예측한 현재 프레임의 회전값
            Quaternion gyroPredictedRotation = currentRotation * gyroDeltaRotation;

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
            currentRotation = filteredAccelCorrection * gyroPredictedRotation;

            //정규화 및 적용
            currentRotation = Quaternion.Normalize(currentRotation);
            transform.rotation = currentRotation;
        }
    }
}