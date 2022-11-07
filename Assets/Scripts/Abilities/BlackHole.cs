using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private float explosionTime;
    [SerializeField] private float influenceRange;
    [SerializeField] private float intensity = 1;

    [SerializeField] private float explosionRadius = 10;
    [SerializeField] private float explosionStrength = 10000f;
    [SerializeField] private float explosionDamage = 50f;

    void Update()
    {
        

    }

    private void BlackHolePull()
    {
        foreach (Rigidbody rigidbody in GravityController.main.rigidbodies)
        {
            Vector3 pullForce;
            float distanceToPlayer;

            distanceToPlayer = Vector3.Distance(rigidbody.position, transform.position);
            if (distanceToPlayer <= influenceRange)
            {
                pullForce = (rigidbody.position - transform.position).normalized * Physics.gravity.y * intensity;
                rigidbody.AddForce(pullForce, ForceMode.Acceleration);
            }

        }
    }

    private void BlackHoleExplode()
    {
        foreach (Rigidbody rigidbody in GravityController.main.rigidbodies)
        {
            CarHealth carHealth = GetComponent<CarHealth>();
            rigidbody.AddExplosionForce(explosionStrength, transform.position, explosionRadius, 100f, ForceMode.Impulse);

            if (carHealth)
            {
                carHealth.AddCarDamage(this.gameObject, HitLocation.BOTTOM, explosionDamage);
            }

        }
    }
}
