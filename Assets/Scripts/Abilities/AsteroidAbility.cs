using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Asteroid")]
public class AsteroidAbility : Ability
{
    public GameObject asteroid;
    private bool activated;
    private bool collided;
    private Rigidbody rb;

    public override void Obtained()
    {
        base.Obtained();

        activated = false;
    }

    public override void Activated()
    {
        base.Activated();

        if (!activated)
        {
            var projectile = Instantiate(asteroid, carController.transform.position + Vector3.up + Vector3.up + Vector3.up + Vector3.up + Vector3.up, carController.transform.rotation);
            rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = carController.transform.TransformDirection(new Vector3(0, 0, 100));
            activated = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (rb)
        {
            collided = rb.GetComponent<AsteroidCollision>().collided;
            if (activated && !collided)
            {
                Debug.Log(collided);
                rb.AddRelativeForce(Vector3.forward * 10);
            }
            else
            {
                activated = false;
                Destroy(rb.gameObject);
            }
        }

    }

    public override void CarDestroyed()
    {
        base.CarDestroyed();
    }
}
