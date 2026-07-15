using UnityEngine;

public class FlightRotationController : MonoBehaviour
{
    [Header("Rotation Speed")]
    [SerializeField] private float rotaionSpeed = 10f;

    [Header("상보 필터(Complementary Filter) 자이로 계수 [0 ~ 1]")]
    [SerializeField] private float gyroWeight = 0.96f;

    MPU6050Data currentData = new MPU6050Data();
    bool isRotationAllowed = true;

    Vector3 gyroRotation = Vector3.zero; //degree per second
    Vector3 accelRotation = Vector3.zero; //degree
    Vector3 filteredRotation = Vector3.zero;

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
        transform.localRotation = Quaternion.identity; //로컬 회전값 0으로 초기화
        filteredRotation = Vector3.zero;
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
            SetGyroRotation();
            SetAccelRotation();

            //Complementary Filter

            // Pitch (Unity X)
            float gyroStepX = gyroRotation.x * Time.deltaTime;
            filteredRotation.x = gyroWeight * (filteredRotation.x + gyroStepX) + (1f - gyroWeight) * accelRotation.x;

            // Yaw (Unity Y) - 예외: 가속도 보정이 없으므로 자이로 100% 누적
            float gyroStepY = gyroRotation.y* Time.deltaTime;
            filteredRotation.y = filteredRotation.y + gyroStepY;

            // Roll (Unity Z)
            float gyroStepZ = gyroRotation.z * Time.deltaTime;
            filteredRotation.z = gyroWeight * (filteredRotation.z + gyroStepZ) + (1f - gyroWeight) * accelRotation.z;

            //수정: 변위 누적 -> 최종 계산된 절대 각도 그대로 대입 (추가 비행기 회전 민감도)
            transform.localRotation = Quaternion.Euler(
            filteredRotation.x * rotaionSpeed,
            filteredRotation.y * rotaionSpeed,
            filteredRotation.z * rotaionSpeed
            );
        }
        

        //디버깅 테스트용, 삭제 예정
        if (Input.GetKeyDown(KeyCode.R) && isRotationAllowed)
        {
            if (isRotationAllowed)
            {
                Debug.Log($"가속도 x{currentData.accelX:F2} y{currentData.accelY:F2} z{currentData.accelZ:F2} / " +
                    $"각속도 x{currentData.gyroX:F2} y{currentData.gyroY:F2} z{currentData.gyroZ:F2}");
            }

            else
            {
                Debug.Log("현재 회전 비행이 허가되지 않았습니다!");
            }
        }
    }

    void SetGyroRotation()
    {
        this.gyroRotation.x = currentData.gyroX * (-1f);
        this.gyroRotation.y = currentData.gyroZ * (-1f);
        this.gyroRotation.z = currentData.gyroY * (-1f);
    }

    void SetAccelRotation()
    {
        //Roll
        float rollRadian = Mathf.Atan2(currentData.accelX, currentData.accelZ);
        float rollDegree = rollRadian * Mathf.Rad2Deg;

        //Pitch
        float pitchRadian = Mathf.Atan2(currentData.accelY, currentData.accelZ);
        float pitchDegree = pitchRadian * Mathf.Rad2Deg;

        this.accelRotation.z = rollDegree;
        this.accelRotation.x = pitchDegree * (-1f);
        this.accelRotation.y = 0f;

    }
}
