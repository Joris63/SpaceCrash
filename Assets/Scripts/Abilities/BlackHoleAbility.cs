using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/BlackHole")]
public class BlackHoleAbility : Ability
{
    [Space(12)]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private int amount = 3;

    private int placedBlackHoles = 0;
    private bool abilityEnded = false;
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

        if (placedBlackHoles < amount)
        {
            placedBlackHoles++;

            BlackHole blackHole = Instantiate(blackHolePrefab, carController.transform.position, Quaternion.identity, abilityController.abilityContainer).GetComponent<BlackHole>();
            blackHole.ActivateBlackHole();

            if (placedBlackHoles >= amount && !abilityEnded)
            {
                abilityEnded = true;
                abilityController.AbilityEnded();
            }
        }
    }

    public override void CarDestroyed()
    {
        base.CarDestroyed();
    }
}
