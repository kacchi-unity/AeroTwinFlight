using UnityEngine;

public class LinkController : MonoBehaviour
{
    private StateMachine StateMachine;

    public LinkIdleState IdleState {  get; private set; }
    public LinkAttackState AttackState { get; private set; }

    void Awake()
    {
        StateMachine = new StateMachine();

        IdleState = new LinkIdleState(this, StateMachine);
        AttackState = new LinkAttackState(this, StateMachine);
    }

    void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    void Update()
    {
        StateMachine.Update();
    }
}
