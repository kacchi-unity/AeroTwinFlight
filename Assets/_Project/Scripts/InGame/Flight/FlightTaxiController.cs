using UnityEngine;

public class FlightTaxiController : MonoBehaviour
{
    [Header("Taxi Target Rigidbody")]
    [SerializeField] private Rigidbody targetRigidbody;

    [Header("Collision Ignore Target")]
    [Header("바퀴 Collider와의 충돌 무시 Collider (예: 몸체)")]
    [SerializeField] private Collider[] ignoreTargetColliderList;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider LeftWheel;
    [SerializeField] private WheelCollider RightWheel;
    [SerializeField] private WheelCollider BackWheel;

    [Header("Taxiing Settings")]
    [Header("최대 바퀴 회전력 (Speed)")]
    [SerializeField] private float maxMotorTorque = 300f;
    [Header("최대 조향 각도 (Turn)")]
    [SerializeField] private float maxSteerAngle = 25f;
    [SerializeField] private float brakeForce = 500f;

    private void Start()
    {
        //targetRigidbody.centerOfMass = new Vector3(0f, -0.2f, 0.5f);
        
        //바퀴와의 사전 충돌 무시 처리 (ex. 동체 Collider)
        WheelCollider[] wheels = { LeftWheel, RightWheel, BackWheel };

        if (ignoreTargetColliderList.Length > 0)
        {
            foreach (Collider ignoreTarget in ignoreTargetColliderList)
            {
                foreach (Collider targetWheel in wheels)
                {
                    Physics.IgnoreCollision(ignoreTarget, targetWheel);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        HandleTaxiing();
    }

    private void HandleTaxiing()
    {
        float inputAccel = Input.GetAxis("Vertical");
        float inputSteer = Input.GetAxis("Horizontal");
        bool isBraking = Input.GetKey(KeyCode.Space);

        //동력
        float currentMotorTorque = inputAccel * maxMotorTorque;
        LeftWheel.motorTorque = currentMotorTorque;
        RightWheel.motorTorque = currentMotorTorque;

        //조향 (Steering)
        BackWheel.steerAngle = inputSteer * maxSteerAngle;

        //브레이크
        float currentBrake = isBraking ? brakeForce : 0f;
        LeftWheel.brakeTorque = currentBrake;
        RightWheel.brakeTorque = currentBrake;
        BackWheel.brakeTorque = currentBrake;
    }
}
