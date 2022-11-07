using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleAbility : Ability
{
    private GameObject firstTarget;
    private GameObject secondTarget;

    public override void Obtained()
    {
        base.Obtained();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void Activated()
    {
        base.Activated();
    }

    public override void CarDestroyed()
    {
        base.CarDestroyed();

        AbilityEnded(true);
    }

    private void AbilityEnded(bool isDestroyed)
    {
        if (!isDestroyed) abilityController.AbilityEnded();
    }
}
