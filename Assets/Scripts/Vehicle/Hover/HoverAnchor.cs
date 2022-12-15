using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HoverAnchor : MonoBehaviour
{
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float springStrength = 25f;
    [SerializeField] private float springDamping = 3f;
    [SerializeField] private float grip = 0.5f;
    [SerializeField] private bool isAccelerator = true;

    private HoverController controller;
    private float torqueForce;
    private float steeringForce;

    private void Start()
    {
        controller = GetComponentInParent<HoverController>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, hoverHeight, controller.ground))
        {
            // Calculate suspension force
            float offset = hoverHeight - hit.distance;
            Vector3 tireVelocity = controller.mainRb.GetPointVelocity(transform.position);
            float desiredVelocity = Vector3.Dot(transform.up, tireVelocity);
            float suspensionForce = (offset * springStrength) - (desiredVelocity * springDamping);

            controller.mainRb.AddForceAtPosition(transform.up * suspensionForce, transform.position);

            // Calculate steering force
            steeringForce = (-Vector3.Dot(transform.right, tireVelocity) * grip) / Time.fixedDeltaTime;

            controller.mainRb.AddForceAtPosition(transform.right * steeringForce, transform.position);

            if (isAccelerator)
            {
                // Calculate acceleration and braking force
                if (controller.verticalInput != 0f)
                {
                    float currentSpeed = Vector3.Dot(controller.mainRb.transform.forward, controller.mainRb.velocity);
                    float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentSpeed) / controller.maxSpeed);
                    torqueForce = controller.torqueCurve.Evaluate(normalizedSpeed) * controller.verticalInput;

                    controller.mainRb.AddForceAtPosition(transform.forward * torqueForce * controller.maxTorquePower, transform.position);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.white;
        Handles.DrawLine(transform.position, transform.position - transform.up * hoverHeight, 10f);

        Handles.color = Color.red;
        Handles.DrawLine(transform.position, transform.position + transform.forward * torqueForce, 10f);
        //Handles.color = Color.green;
        //Handles.DrawLine(transform.position, transform.position + transform.right * steeringForce / 100f, 10f);
    }
#endif
}
