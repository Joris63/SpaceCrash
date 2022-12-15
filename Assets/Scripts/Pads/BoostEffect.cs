using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pads/Boost")]
public class BoostEffect : PadEffect
{
    public float boostMultiplier = 1.5f;
    public float boostDuration = 1.5f;

    public override void ApplyEffect(Rigidbody car)
    {
        base.ApplyEffect(car);

        // apply boost
        CarController carController = car.GetComponent<CarController>();

        car.velocity *= boostMultiplier;
        carController.boostDeactivateTime = carController.timeAlive + boostDuration;
    }
}

