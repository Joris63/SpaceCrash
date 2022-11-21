using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Asteroid")]
public class AsteroidAbility : Ability
{
    private Transform car;
    public GameObject asteroid;
    private bool readytoThrow;

    public override void Obtained()
    {
        base.Obtained();

        readytoThrow = true;
    }

    public override void Activated()
    {
        base.Activated();

        if (readytoThrow)
        {
            Throw();
        }
    }

    private void Throw()
    {
        readytoThrow = false;
        Instantiate(asteroid, car.position, Quaternion.identity);
    }


}
