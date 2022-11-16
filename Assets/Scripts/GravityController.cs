using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public static GravityController main;

    public List<Rigidbody> rigidbodies { get; private set; } = new List<Rigidbody>();

    private void Awake()
    {
        main = this;
        rigidbodies = FindObjectsOfType<Rigidbody>().ToList();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    public void AddRigidbody(Rigidbody rb)
    {
        rigidbodies.Add(rb);
    }

    private void ApplyGravity()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            Vector3 direction = (transform.position - rb.transform.position).normalized;

            rb.AddForce(direction * Physics.gravity.y, ForceMode.Acceleration);
        }
    }
}
