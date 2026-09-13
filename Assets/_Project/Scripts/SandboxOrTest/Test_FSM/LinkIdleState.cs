using UnityEngine;

public class LinkIdleState : IState
{
    private LinkController link;
    private StateMachine stateMachine;

    public LinkIdleState(LinkController link, StateMachine stateMachine)
    {
        this.link = link;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Idle 상태 시작");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateMachine.ChangeState(link.AttackState);
        }
    }

    public void Exit()
    {
        Debug.Log("Idle 상태 종료");
    }

    public void FixedUpdate()
    {

    }
}
