using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlightHUD : MonoBehaviour
{
    [Header("정보 출력 UI Text")]
    [SerializeField] private TextMeshProUGUI generalInfoText;
    [SerializeField] private TextMeshProUGUI forceInfoText;
    [SerializeField] private TextMeshProUGUI rotationInfoText;

    [Header ("General 정보 컴포넌트")]
    [SerializeField] private FlightStateController flightStateController;
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private WheelCollider backWheelCollider;

    [Header("Force 정보 컴포넌트")]
    [SerializeField] private Slider engineSlider; //임시
    [SerializeField] private EngineController engineController;
    [SerializeField] private BrakeController brakeController;
    [SerializeField] private FlightLiftController flightLiftController;
    [SerializeField] private CLController clController;
    [SerializeField] private FlightDragController flightDragController;

    [Header("데이터 UI 업데이트 주기 (초)")]
    [SerializeField] private float updatePeriod = 0.1f;

    private string currentStateName = null;
    private float initialHeight;
    private float deltaHeight;
    private Quaternion absoluteQuaternion = Quaternion.identity;
    private Quaternion attitudeControlQuaternion = Quaternion.identity;

    //optimization frame string GC
    private float timeSinceUpdate = 0f;

    private void OnEnable()
    {
        flightStateController.StateChanged += SaveCurrentState;

        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated += SaveAbsoluteQuaternion;
        SensorQuaternionCalculator.OnAttitudeControlQuaternionCalculated += SaveAttitudeControlQuaternion;

    }

    private void OnDisable()
    {
        flightStateController.StateChanged -= SaveCurrentState;

        SensorQuaternionCalculator.OnAbsoluteQuaternionCalculated -= SaveAbsoluteQuaternion;
        SensorQuaternionCalculator.OnAttitudeControlQuaternionCalculated -= SaveAttitudeControlQuaternion;

    }

    //Save Data
    private void SaveCurrentState(IState targetState)
    {
        this.currentStateName = targetState.GetStateName();

        if (targetState is TakeoffState && targetTransform != null)
        {
            this.initialHeight = targetTransform.position.y;
        }
    }

    void SaveAbsoluteQuaternion(Quaternion quaternionData)
    {
        this.absoluteQuaternion = quaternionData;
    }

    void SaveAttitudeControlQuaternion(Quaternion quaternionData)
    {
        this.attitudeControlQuaternion = quaternionData;
    }

    void Update()
    {
        timeSinceUpdate += Time.deltaTime;
        if (timeSinceUpdate >= updatePeriod)
        {
            PrintGeneralInformation();
            PrintForceInformation();
            PrintRotationInformation();

            timeSinceUpdate = 0f;
        }
        
    }

    private void FixedUpdate()
    {
        this.deltaHeight = targetTransform.position.y - this.initialHeight;
    }

    void PrintGeneralInformation()
    {
        switch (currentStateName)
        {
            case nameof(CalibratingState):
                generalInfoText.text =
                    $"[ 보정 상태 ({nameof(CalibratingState)}) ]\n" +
                    "  보정 중...";
                break;

            case nameof(TaxiingState):
                generalInfoText.text =
                    $"[ 지상 주행 상태 ({nameof(TaxiingState)}) ]\n" +
                    $"  속도: {targetRigidbody.linearVelocity.magnitude:F1} m/s\n" +
                    $"  이륙 가능 속도: {flightStateController.GetTakeoffSpeed():F1} m/s 이상\n" +
                    $"  조향각: {backWheelCollider.steerAngle:F1}°";
                break;

            case nameof(TakeoffState):
                generalInfoText.text =
                    $"[ 이륙 가능 상태 ({nameof(TakeoffState)}) ]\n" +
                    $"  속도: {targetRigidbody.linearVelocity.magnitude:F1} m/s\n" +
                    $"  고도: {deltaHeight:F1} m\n" +
                    $"  기수 각도(Pitch): {(-1f)* GetInspectorAngle(targetTransform.eulerAngles.x):F1}°\n" +
                    $"  기수를 위로 올릴 시 비행 가능";
                break;

            case nameof(FlyingState):
                generalInfoText.text =
                    $"[ 비행 상태 ({nameof(FlyingState)}) ]\n" +
                    $"  속도: {targetRigidbody.linearVelocity.magnitude:F1} m/s\n" +
                    $"  고도: {deltaHeight:F1} m\n";
                break;

            case nameof(LandingState):
                generalInfoText.text =
                    $"[ 착륙 상태 ({nameof(LandingState)}) ]\n" +
                    $"  속도: {targetRigidbody.linearVelocity.magnitude:F1} m/s\n" +
                    $"  고도: {deltaHeight:F1} m\n"+
                    $"  착륙 가능 속도: {flightStateController.GetTaxiingTransitionSpeed():F1} m/s 이하\n"+
                    $"  속도를 줄이세요";
                break;

            default:
                generalInfoText.text = "No Information";
                break;
        }
    }

    void PrintForceInformation()
    {
        forceInfoText.text =
            $"[엔진 (ENGINE)]\n" +
            $"엔진 출력: {engineSlider.value * 100:F1} %\n" +
            $"엔진 힘: {engineController.GetCurrentEnginePower():F1} N\n" +
            $"브레이크: {brakeController.GetCurrentBrakeForce():F1} N\n" +
            "\n" +
            $"[공력 (AERODYNAMICS)]\n" +
            $"양력 (Lift): {flightLiftController.GetLift():F1} N\n" +
            $"받음각 (AOA): {clController.GetAOA():F1}°\n" +
            $"양력 계수 (CL): {clController.GetCL():F1}\n" +
            $"항력 (Drag): {flightDragController.GetDrag():F1} N\n";
    }

    void PrintRotationInformation()
    {
        Vector3 currentAbsAngles = Quaternion2Vector(this.absoluteQuaternion);
        Vector3 currentACAngles = Quaternion2Vector(this.attitudeControlQuaternion);

        rotationInfoText.text =
            $"          x         y        z\n" +
            $"ABS  {currentAbsAngles.x,6:F1}° {currentAbsAngles.y,6:F1}° {currentAbsAngles.z,6:F1}°\n" +
            $"AC   {currentACAngles.x,6:F1}° {currentACAngles.y,6:F1}° {currentACAngles.z,6:F1}°";
    }

    //쿼터니안 -> X, Y, Z 각도 정보 (Vector3) 반환 (GetInspectorAngle 사용)
    private Vector3 Quaternion2Vector(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;

        return new Vector3(
            GetInspectorAngle(euler.x),
            GetInspectorAngle(euler.y),
            GetInspectorAngle(euler.z)
        );
    }

    //0 ~ 360 범위를 -180 ~ 180 범위로 변환
    private float GetInspectorAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

}
