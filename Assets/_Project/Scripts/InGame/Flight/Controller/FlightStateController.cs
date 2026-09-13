using UnityEngine;

public class FlightStateController : MonoBehaviour
{
    //MonoBehaviour Object
    [SerializeField] private FlightTaxiController flightTaxiController;
    
    //State Machine
    private StateMachine stateMachine;

    //State List
    public TaxiingState TaxiingState { get; private set; }
    public FlyingState FlyingState { get; private set; }
    public CalibratingState CalibratingState { get; private set; }

    //Property List
    public IState CurrentState => stateMachine.CurrentState;
    public IState PreviousState => stateMachine.PreviousState;

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

        //State List
        TaxiingState = new TaxiingState(this, stateMachine, flightTaxiController);
        FlyingState = new FlyingState(this, stateMachine);
        CalibratingState = new CalibratingState(this, stateMachine, flightTaxiController);
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
            stateMachine.ChangeState(CalibratingState);
        }
    }

    void FinishCalibrate()
    {
        if (PreviousState != null && CurrentState == CalibratingState)
        {
            stateMachine.ChangeState(PreviousState);
            return;
        }

        Debug.LogWarning($"{this.name}: Previous 또는 Current 상태를 확인하세요.");
    }
}
