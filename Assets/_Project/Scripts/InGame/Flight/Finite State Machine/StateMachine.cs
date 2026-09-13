public class StateMachine
{
    public IState CurrentState { get; private set; }
    public IState PreviousState { get; private set; }

    public void Initialize(IState targetState)
    {
        PreviousState = null;

        CurrentState = targetState;
        CurrentState.Enter();
    }

    public void ChangeState(IState targetState)
    {
        PreviousState = CurrentState;

        CurrentState.Exit();
        CurrentState = targetState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState.Update();
    }

    public void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }
}
