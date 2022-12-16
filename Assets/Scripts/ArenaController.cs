using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AffectedObject
{
    public float timeInCenterSphere = 0f;
    public Rigidbody rigidbody = null;
    public CarHealth carHealth = null;
}

public class ArenaController : MonoBehaviour
{
    public static ArenaController main;

    public GameObject Outside;
    public GameObject Inside;

    [Header("Healing Sphere")]
    public AnimationCurve speedMultiplierCurve;
    public AnimationCurve rotationSpeedMultiplierCurve;
    public bool inverted { get; private set; } = false;

    private float healingSphereRadius;

    private List<AffectedObject> rigidbodies = new List<AffectedObject>();

    private void Awake()
    {
        main = this;

        foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        {
            AddRigidbody(rb);
        }

        healingSphereRadius = Inside.GetComponent<SphereCollider>().radius * Inside.transform.localScale.x;
    }

    private void Update()
    {
        ApplyHeal();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    public void AddRigidbody(Rigidbody rb)
    {
        rigidbodies.Add(new AffectedObject() { rigidbody = rb, carHealth = rb.GetComponent<CarHealth>() });
    }

    private void ApplyGravity()
    {
        foreach (AffectedObject obj in rigidbodies)
        {
            Vector3 direction = (transform.position - obj.rigidbody.transform.position).normalized;

            obj.rigidbody.AddForce(direction * Physics.gravity.y * (inverted ? -1 : 1) * (obj.timeInCenterSphere > 0 ? .3f : 1), ForceMode.Acceleration);
        }
    }

    private void ApplyHeal()
    {
        foreach (AffectedObject obj in rigidbodies)
        {
            float distanceFromCenter = (obj.rigidbody.transform.position - transform.position).magnitude;

            if (distanceFromCenter <= healingSphereRadius)
            {
                obj.timeInCenterSphere += Time.deltaTime;

                // Limit speed and rotation speed
                obj.rigidbody.velocity = (speedMultiplierCurve.Evaluate(obj.timeInCenterSphere) / 3.6f) * obj.rigidbody.velocity.normalized;
                obj.rigidbody.angularVelocity = (rotationSpeedMultiplierCurve.Evaluate(obj.timeInCenterSphere) / 3.6f) * obj.rigidbody.angularVelocity.normalized;

                if (obj.carHealth != null && !obj.carHealth.isHealing)
                {
                    obj.carHealth.StartHealing();
                }
            }
            else if (obj.carHealth != null && obj.carHealth.isHealing)
            {
                if (obj.timeInCenterSphere > 0)
                    obj.rigidbody.velocity *= 1.5f;

                obj.carHealth.StopHealing();
                obj.timeInCenterSphere = 0;
            }
        }
    }
}
