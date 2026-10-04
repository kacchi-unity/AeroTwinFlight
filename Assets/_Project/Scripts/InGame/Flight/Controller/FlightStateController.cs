using System;
using UnityEngine;

public class FlightStateController : MonoBehaviour
{
    [Header ("컨트롤러 컴포넌트")]
    [SerializeField] private EngineController flightEngineController;
    [SerializeField] private TaxiingController taxiingController;
    [SerializeField] private TakeoffController takeoffController;
    [SerializeField] private FlyingController flyingController;
    [SerializeField] private BrakeController brakeController;

    [Header("상태 머신 필요 데이터")]
    [Tooltip("물리 Rigidbody")]
    [SerializeField] private Rigidbody flightRidigbody;

    [Tooltip("바닥 감지 콜라이더 등록")]
    [SerializeField] private BoxCollider[]groundCheckColliders;

    [Tooltip("비행 전 지상 주행 Taxiing 시 Takeoff 상태 전환 임계 속력 값")]
    [SerializeField] private float takeoffSpeed = 28.7f;

    [Tooltip("Landing 상태에서 Taxiing 상태 전환에 필요한 줄임 속력 값")]
    [SerializeField] private float taxiingTransitionSpeed = 6f;

    [Tooltip("Ground 레이어 태그 등록")]
    [SerializeField] private LayerMask groundLayer;

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
    public event Action BothTriggerGrounded;
    public event Action BothTriggerAirborne;

    //Global Variables
    private TriggerDetector frontGroundDetector;
    private TriggerDetector backGroundDetector;


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
            flightEngineController,
            taxiingController
            );

        TakeoffState = new TakeoffState(
            this,
            taxiingController,
            takeoffController,
            flightRidigbody,
            takeoffSpeed
            );

        FlyingState = new FlyingState(
            this, 
            flyingController,
            brakeController
            );

        LandingState = new LandingState(
            this,
            taxiingController,
            flightRidigbody,
            taxiingTransitionSpeed
            );

        SetupTriggerDetector();
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

    //Ground Trigger Collider Evnet Processing
    private void SetupTriggerDetector()
    {
        if (groundCheckColliders == null || groundCheckColliders.Length < 2)
        {
            Debug.LogError($"{this.name}: groundCheckColliders가 2개 이상 등록되지 않았습니다!");
            return;
        }

        frontGroundDetector = SetupDetector(groundCheckColliders[0]);
        backGroundDetector = SetupDetector(groundCheckColliders[1]);

        if (frontGroundDetector != null && backGroundDetector != null)
        {
            frontGroundDetector.TouchStateChanged += CheckAllGroundTrigger;
            backGroundDetector.TouchStateChanged += CheckAllGroundTrigger;
        }
    }

    private TriggerDetector SetupDetector(BoxCollider box)
    {
        box.isTrigger = true;

        var detector = box.GetComponent<TriggerDetector>();

        if (detector == null)
        {
            detector = box.gameObject.AddComponent<TriggerDetector>();
        }

        detector.Initialize(this.groundLayer);

        return detector;
    }

    private void CheckAllGroundTrigger(bool isTouching)
    {
        if (frontGroundDetector!= null && backGroundDetector != null)
        {
            if (frontGroundDetector.IsTouchingTarget && backGroundDetector.IsTouchingTarget)
            {
                this.BothTriggerGrounded?.Invoke();
            }

            else if (!frontGroundDetector.IsTouchingTarget && !backGroundDetector.IsTouchingTarget)
            {
                this.BothTriggerAirborne?.Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        frontGroundDetector.TouchStateChanged -= CheckAllGroundTrigger;
        backGroundDetector.TouchStateChanged -= CheckAllGroundTrigger;
    }

    public float GetTakeoffSpeed()
    {
        return this.takeoffSpeed;
    }

    public float GetTaxiingTransitionSpeed()
    {
        return this.taxiingTransitionSpeed;
    }
}
