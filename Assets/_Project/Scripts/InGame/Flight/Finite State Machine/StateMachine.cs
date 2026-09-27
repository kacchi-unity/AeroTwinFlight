public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void Initialize(IState targetState)
    {
        CurrentState = targetState;
        CurrentState.Enter();
    }

    public void ChangeState(IState targetState)
    {
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
