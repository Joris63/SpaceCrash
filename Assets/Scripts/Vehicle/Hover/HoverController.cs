using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    public float maxSpeed = 20f;
    public float maxTorquePower = 60f;
    public AnimationCurve torqueCurve;
    [SerializeField] private List<WheelAnchor> steeringAnchors;
    public LayerMask ground;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private bool isBot = false;

    [HideInInspector] public Rigidbody mainRb;
    [HideInInspector] public float verticalInput = 0f;
    [HideInInspector] public float horizontalInput = 0f;

    private void Awake()
    {
        mainRb = GetComponent<Rigidbody>();
        mainRb.centerOfMass = centerOfMass.localPosition;
    }

    private void Update()
    {
        if (!isBot)
        {
            // Set current acceleration direction
            verticalInput = Input.GetAxisRaw("Vertical");
            // Set current steering direction
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }

        // Set rotation of steering anchors based on horizontal input
        foreach (WheelAnchor anchor in steeringAnchors)
        {
            float desiredRot = horizontalInput == 0 ? 0f : horizontalInput == -1 ? -30f : 30f;
            anchor.transform.localEulerAngles = new Vector3(0, desiredRot, 0);
        }

        // Slow down the car if no vertical input is present
        mainRb.drag = verticalInput == 0 ? 1f : 0f;
    }
}
