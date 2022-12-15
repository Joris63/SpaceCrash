using UnityEngine;

public class Reversing : BaseState
{
    private float reverseTime;

    public Reversing(CarAI carAI) : base(carAI) { }

    public override void Enter()
    {
        base.Enter();

        reverseTime = Random.Range(1f, 2f);

        // Set acceleration direction
        carAI.SetVertical(-1f);

        // Set steering direction
        carAI.SetHorizontal(0f);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (carAI.currentState != this) return;

        reverseTime -= Time.deltaTime;

        // Transition
        if (reverseTime <= 0f) carAI.ChangeState(carAI.idle);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
