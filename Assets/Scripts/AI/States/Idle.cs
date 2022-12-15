using UnityEngine;

public class Idle : BaseState
{
    private float randomOffset;

    public Idle(CarAI carAI) : base(carAI) { }

    public override void Enter()
    {
        base.Enter();

        randomOffset = Random.Range(0f, 999f);

        // Set acceleration direction
        carAI.SetVertical(1f);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (carAI.currentState != this) return;

        // Handle abilities
        carAI.useAbilityCooldown -= Time.deltaTime;
        if (carAI.useAbilityCooldown <= 0f)
        {
            carAI.useAbilityCooldown = Random.Range(3f, 10f);
            abilityController.UseAbility();
        }

        // Smooth random number between -1 and 1 using Perlin noise
        float steerValue = Mathf.PerlinNoise(Time.time + randomOffset, 0f) * 2f - 1f;

        // Set steering direction
        float horizontal = Mathf.Abs(steerValue) > 0.2f ? Mathf.Sign(steerValue) : 0f;
        carAI.SetHorizontal(horizontal);

        // Transition
        carAI.idleTime -= Time.deltaTime;
        if (carAI.hitOpponent)
        {
            carAI.hitOpponent = false;
            carAI.ChangeState(carAI.reversing);
        }
        else if (carAI.idleTime <= 0f) carAI.ChangeState(carAI.pursuing);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
