using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("Black hole Properties")]
    [SerializeField] private float influenceRange;
    [SerializeField] private float intensity = 1;
    [SerializeField] private float activationTime;
    [SerializeField] private float pullTime;


    [Header("Explosion properties")]
    [SerializeField] private float explosionRadius = 10;
    [SerializeField] private float explosionStrength = 10000f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private float explosionSoundVolume;

    [Header("Debugging")]
    [SerializeField] private bool activate = false;

    private bool isActivated = false;
    private float timeRunning;


    void Update()
    {
        timeRunning += Time.deltaTime;

        if (activate)
        {
            activate = false;
            timeRunning = 0;
            ActivateBlackHole();
        }

        if (isActivated)
        {
            if (timeRunning >= activationTime + pullTime)
            {
                isActivated = false;
                BlackHoleExplode();
            }
            else if (timeRunning >= activationTime)
            {
                BlackHolePull();
            }
        }

    }

    public void ActivateBlackHole()
    {
        if (!isActivated)
        {
            isActivated = true;
        }
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
            rigidbody.AddExplosionForce(explosionStrength, transform.position, explosionRadius, 0f, ForceMode.VelocityChange);
            //AudioController.main.PlayOneShot(gameObject.transform.position, explosionAudioClip, 1f, explosionSoundVolume);

            if (carHealth)
            {
                carHealth.AddCarDamage(this.gameObject, HitLocation.BOTTOM, explosionDamage);
            }
        }
        Destroy(gameObject);
    }
}
