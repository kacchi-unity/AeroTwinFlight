using UnityEngine;
using UnityEngine.UIElements;

public class FlightRotationController : MonoBehaviour
{
    [Header("Rotation Speed")]
    [SerializeField] private float rotaionSpeed = 10f;

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
        transform.localRotation = Quaternion.identity; //로컬 회전값 0으로 초기화
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
            float rotationX = currentData.gyroX * rotaionSpeed;
            float rotationY = currentData.gyroY * rotaionSpeed;
            float rotationZ = currentData.gyroZ * rotaionSpeed;

            Quaternion deltaRotation = Quaternion.Euler(
                -rotationX * Time.deltaTime,
                -rotationZ * Time.deltaTime,
                -rotationY * Time.deltaTime
                );

            transform.rotation = transform.rotation * deltaRotation;
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
}
