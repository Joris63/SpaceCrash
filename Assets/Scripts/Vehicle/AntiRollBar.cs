using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [SerializeField] private WheelAnchor leftAnchor;
    [SerializeField] private WheelAnchor rightAnchor;
    [SerializeField] private float antiRoll = 5000f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float travelL = 1f;
        float travelR = 1f;

        if (leftAnchor.isGrounded)
        {
            travelL = (-leftAnchor.transform.InverseTransformPoint(leftAnchor.hit.point).y - leftAnchor.hoverHeight) / (leftAnchor.hoverHeight / 2f);
        }

        if (rightAnchor.isGrounded)
        {
            travelR = (-rightAnchor.transform.InverseTransformPoint(rightAnchor.hit.point).y - rightAnchor.hoverHeight) / (rightAnchor.hoverHeight / 2f);
        }

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (leftAnchor.isGrounded)
        {
            rb.AddForceAtPosition(leftAnchor.transform.up * -antiRollForce, leftAnchor.transform.position);
        }

        if (leftAnchor.isGrounded)
        {
            rb.AddForceAtPosition(rightAnchor.transform.up * antiRollForce, rightAnchor.transform.position);
        }
    }
}
