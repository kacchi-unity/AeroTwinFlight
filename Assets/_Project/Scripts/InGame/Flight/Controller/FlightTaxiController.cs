using UnityEngine;

public class FlightTaxiController : MonoBehaviour
{
    [Header("Target Rigidbody")]
    [SerializeField] private Rigidbody targetRigidbody;
    [Header("Y축 무게중심 설정 고정값 (브레이크 후륜 이탈 방지)")]
    [SerializeField] private float centerOfMassYOffset = -1f;

    [Header("바퀴 Collider와의 충돌 무시 Collider 대상 (예: 몸체)")]
    [SerializeField] private Collider[] ignoreTargetColliderList;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider LeftWheel;
    [SerializeField] private WheelCollider RightWheel;
    [SerializeField] private WheelCollider BackWheel;

    [Header("Taxiing Settings")]
    [Header("최대 바퀴 회전력 (Speed)")]
    [Min(1f)] [SerializeField] private float maxMotorTorque = 1000f;
    [Header("최대 조향 각도 (Turn)")]
    [Min(1f)] [SerializeField] private float maxSteerAngle = 50f;
    [Header("브레이크 정지 출력 힘")]
    [Min(1f)] [SerializeField] private float brakeForce = 3000f;

    //현재 바퀴가 실제로 꺾여 있는 각도
    private float currentSteerAngle = 0f;

    //ABS 모드 저장 Yaw
    private float currentAbsoluteYaw = 0f;

    //Event 
    private void OnEnable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated += UpdateRotation;
    }

    private void OnDisable()
    {
        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated -= UpdateRotation;
    }

    private void Awake()
    {
        if (targetRigidbody == null)
        {
            Debug.LogWarning($"{this.name}: Null 확인 필요");
            return;
        }

        //바퀴와의 사전 충돌 무시 처리 (ex. 동체 Collider)
        WheelCollider[] wheels = { LeftWheel, RightWheel, BackWheel };

        if (ignoreTargetColliderList != null && ignoreTargetColliderList.Length > 0)
        {
            foreach (Collider ignoreTarget in ignoreTargetColliderList)
            {
                foreach (Collider targetWheel in wheels)
                {
                    Physics.IgnoreCollision(ignoreTarget, targetWheel);
                }
            }
        }

        //Ridig Body 내 무게 중심 조절 (브레이크 뒷 바퀴 쏠림 방지)
        Vector3 centerOfMass = targetRigidbody.centerOfMass;
        centerOfMass.y = this.centerOfMassYOffset;
        targetRigidbody.centerOfMass = centerOfMass;
    }

    void UpdateRotation(Quaternion currentRotation)
    {
        currentAbsoluteYaw = currentRotation.eulerAngles.y;
    }

    public void StartCalibration()
    {
        ResetPhysics();
    }

    private void ResetPhysics()
    {
        if (targetRigidbody != null)
        {
            targetRigidbody.linearVelocity = Vector3.zero;
            targetRigidbody.angularVelocity = Vector3.zero;
        }

        if (LeftWheel != null) { LeftWheel.motorTorque = 0f; LeftWheel.brakeTorque = 0f; LeftWheel.steerAngle = 0f; }
        if (RightWheel != null) { RightWheel.motorTorque = 0f; RightWheel.brakeTorque = 0f; RightWheel.steerAngle = 0f; }
        if (BackWheel != null) { BackWheel.motorTorque = 0f; BackWheel.brakeTorque = 0f; BackWheel.steerAngle = 0f; }

        currentSteerAngle = 0f;

        Debug.Log("Taxiing: 물리 상태와 조향 값이 처음 상태로 초기화되었습니다.");
    }

    //Taxiing State Reference (public/private)
    public void UpdateTaxiing()
    {
        ProcessTaxiing();
    }

    private void ProcessTaxiing()
    {
        //동력
        float motorTorque = Input.GetKey(KeyCode.W) ? maxMotorTorque : 0f;

        LeftWheel.motorTorque = motorTorque;
        RightWheel.motorTorque = motorTorque;

        // 조향 (Steering) : Absolute Mode Yaw Rotation 사용
        // -180 ~ 180도 내 오일러 Yaw각도 자동 정규화 (0f로 부터 최단거리 각도)
        float normalizedYaw = Mathf.DeltaAngle(0f, currentAbsoluteYaw);

        // 최대 조향 각도 제한
        currentSteerAngle = Mathf.Clamp(normalizedYaw, -maxSteerAngle, maxSteerAngle);

        BackWheel.steerAngle = currentSteerAngle * (-1);

        //브레이크 제어
        float brake = Input.GetKey(KeyCode.S) ? brakeForce : 0f;

        LeftWheel.brakeTorque = brake;
        RightWheel.brakeTorque = brake;
        BackWheel.brakeTorque = brake;

    }
}
