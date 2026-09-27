using System;
using UnityEngine;

public class FlightStateController : MonoBehaviour
{
    [Header ("컨트롤러 컴포넌트")]
    [SerializeField] private EngineController flightEngineController;
    [SerializeField] private TaxiingController taxiingController;
    [SerializeField] private FlyingController flyingController;

    [Header("상태 머신 필요 데이터")]
    [Tooltip("물리 Rigidbody")]
    [SerializeField] private Rigidbody flightRidigbody;

    [Tooltip("바퀴 콜라이더 등록")]
    [SerializeField] private WheelCollider[] wheelColliders;

    [Tooltip("비행 전 지상 주행 Taxiing 시 Takeoff 상태 전환 임계 속도 값")]
    [SerializeField] private float takeoffSpeed = 28.7f;

    [Tooltip("Takeoff 시작 후 Flying 이륙 상태 전환에 필요한 고도 상승량")]
    [SerializeField] private float takeoffHeightDelta = 0.5f;

    //State Machine
    private StateMachine stateMachine;

    //State Instance Variables (선언부)
    public TaxiingState TaxiingState { get; private set; }
    public FlyingState FlyingState { get; private set; }
    public CalibratingState CalibratingState { get; private set; }
    public TakeoffState TakeoffState { get; private set; }
    public LandingState LandingState { get; private set; }

    //Property List
    public IState CurrentState => stateMachine.CurrentState;
    private IState stateBeforeCalibration;

    //Event
    public event Action<IState> StateChanged; 


    private void OnEnable()
    {
        CalibrateManager.onStartCalibrate += StartCalibrate;
        CalibrateManager.onFinishCalibrate += FinishCalibrate;
    }

    private void OnDisable()
    {
        CalibrateManager.onStartCalibrate -= StartCalibrate;
        CalibrateManager.onFinishCalibrate -= FinishCalibrate;
    }

    void Awake()
    {
        //State Machine
        stateMachine = new StateMachine();

        //Instantiate State Object (인스턴스 생성부)
        TaxiingState = new TaxiingState(
            this,
            taxiingController,
            flightRidigbody,
            takeoffSpeed
            );

        CalibratingState = new CalibratingState(
            this,
            flightEngineController,
            taxiingController
            );

        TakeoffState = new TakeoffState(
            this,
            flyingController,
            flightRidigbody,
            takeoffSpeed,
            takeoffHeightDelta
            );

        FlyingState = new FlyingState(
            this, 
            flyingController,
            wheelColliders
            );

        LandingState = new LandingState(
            this
            );
    }

    void Start()
    {
        stateMachine.Initialize(TaxiingState);
    }

    void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    void StartCalibrate()
    {
        //중복 보정 호출 무시
        if (CurrentState != CalibratingState)
        {
            stateBeforeCalibration = CurrentState;

            ChangeState(CalibratingState);

        }
    }

    void FinishCalibrate()
    {
        if (stateBeforeCalibration != null && CurrentState == CalibratingState)
        {
            ChangeState(stateBeforeCalibration);
            return;
        }

        Debug.LogWarning($"{this.name}: Previous 또는 Current 상태를 확인하세요.");
    }

    public void ChangeState(IState nextState)
    {
        stateMachine.ChangeState(nextState);
        StateChanged?.Invoke(nextState);
        
    }
}
