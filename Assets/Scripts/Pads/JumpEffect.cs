using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pads/Jump")]
public class JumpEffect : PadEffect
{
    public float jumpForce = 10f;

    public override void ApplyEffect(Rigidbody car)
    {
        base.ApplyEffect(car);

        // apply jump
        CarController carController = car.GetComponent<CarController>();
        car.AddForce(-carController.downDirection * jumpForce, ForceMode.VelocityChange);
    }
}
