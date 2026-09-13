using UnityEngine;

public class LinkAttackState : IState
{
    private LinkController link;
    private StateMachine stateMachine;

    public LinkAttackState(LinkController link, StateMachine stateMachine)
    {
        this.link = link;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Attack 상태 시작");
    }

    public void Update()
    {
        link.transform.Rotate(0f, 180f * Time.deltaTime, 0f);

        if (Input.GetKeyDown(KeyCode.A))
        {
            stateMachine.ChangeState(link.IdleState);
        }
    }

    public void Exit()
    {
        Debug.Log("Attakc 상태 종료");
    }
    
    public void FixedUpdate()
    {

    }
}
