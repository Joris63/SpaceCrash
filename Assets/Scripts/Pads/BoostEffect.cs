using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pads/Boost")]
public class BoostEffect : PadEffect
{
    public override void ApplyEffect(Rigidbody car)
    {
        base.ApplyEffect(car);

        // apply boost
    }
}

