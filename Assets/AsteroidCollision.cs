using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
    public bool collided;
    // Start is called before the first frame update
    private void Start()
    {
        collided = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name != "Inside")
        {
            collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(10000, transform.position, 18, 100f, ForceMode.Impulse);

            collided = true;
        }
    }
}
