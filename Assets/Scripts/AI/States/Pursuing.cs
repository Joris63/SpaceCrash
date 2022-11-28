using UnityEngine;

public class Pursuing : BaseState
{
    private CarController currentTarget;
    private Transform whitelistedTarget;
    private float targetTime = 0f;
    private float whitelistedTime = 0f;

    public Pursuing(CarAI carAI) : base(carAI) { }

    public override void Enter()
    {
        base.Enter();

        currentTarget = null;
        whitelistedTarget = null;
        targetTime = 0f;

        // Set acceleration direction
        carAI.SetVertical(1f);

        // Set steering direction
        carAI.SetHorizontal(0f);
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

        // Assign target
        float closestCar = 999f;
        CarController newTarget = null;

        if (currentTarget != null && (!currentTarget.isTargetable || currentTarget.isDestroyed)) currentTarget = null;
        foreach (CarController car in carAI.cars)
        {
            if (car.transform == carAI.transform || !car.gameObject.activeSelf || car.transform == whitelistedTarget || car.isDestroyed || !car.isTargetable) continue;

            bool isWithinView = Mathf.Abs(Vector3.SignedAngle(carAI.transform.forward, (car.transform.position - carAI.transform.position).normalized, Vector3.up)) <= 60f;

            float distance = (car.transform.position - carAI.transform.position).magnitude;
            if (isWithinView && distance < closestCar)
            {
                closestCar = distance;
                newTarget = car;
            }
        }

        #region Handle Target Correctly Updating
        Transform newWhitelisted = null;

        // If the target is still the same
        if (newTarget != null && newTarget.rb == currentTarget)
        {
            targetTime += Time.deltaTime;

            if (targetTime >= 10f)
            {
                newWhitelisted = currentTarget.transform;
                currentTarget = null;
            }
        }
        // If the target is not the same
        else
        {
            // If there is no target
            if (newTarget == null)
            {
                currentTarget = null;
            }
            // If there was no target or the previous target is different from the new target
            else if (currentTarget == null || newTarget.transform != currentTarget.transform)
            {
                currentTarget = newTarget;
            }

            targetTime = 0f;
        }

        // If the whitelisted target is still the same
        if (newWhitelisted != null && newWhitelisted == whitelistedTarget)
        {
            whitelistedTime += Time.deltaTime;

            if (whitelistedTime >= 5f)
            {
                whitelistedTarget = null;
            }
        }
        // If the whitelisted target is not the same
        else
        {
            whitelistedTarget = newWhitelisted;
            whitelistedTime = 0;
        }
        #endregion

        // Set steering direction
        if (currentTarget)
        {
            float predictValue = Mathf.Clamp(currentTarget.rb.velocity.magnitude / 3f, 0f, (currentTarget.transform.position - carAI.transform.position).magnitude);
            float playerSideLR = Vector3.SignedAngle(carAI.transform.forward, (currentTarget.transform.position + currentTarget.transform.forward * predictValue - carAI.transform.position).normalized, carAI.transform.up);

            if (Mathf.Abs(playerSideLR) > 10f)
            {
                carAI.SetHorizontal(Mathf.Sign(playerSideLR));
            }
            else
            {
                carAI.SetHorizontal(0f);
            }
        }

        // Transition
        if (carAI.hitOpponent)
        {
            carAI.hitOpponent = false;

            float idleValue = Random.Range(0f, 1f);
            if (idleValue > carAI.aggression) carAI.idleTime = Random.Range(5f, 10f);

            carAI.ChangeState(carAI.reversing);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
