using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WheelAnchor : MonoBehaviour
{
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float springStrength = 25f;
    [SerializeField] private float springDamping = 3f;
    [SerializeField] private float grip = 0.5f;
    [SerializeField] private bool isAccelerator = true;

    public Transform wheelView { get; private set; }

    public bool isGrounded { get; private set; }

    private CarController controller;
    private float torqueForce;
    private float steeringForce;

    private void Start()
    {
        controller = GetComponentInParent<CarController>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, hoverHeight, controller.ground))
        {
            isGrounded = true;

            // Calculate suspension force
            float offset = hoverHeight - hit.distance;
            Vector3 tireVelocity = controller.rb.GetPointVelocity(transform.position);
            float desiredVelocity = Vector3.Dot(transform.up, tireVelocity);
            float suspensionForce = (offset * springStrength) - (desiredVelocity * springDamping);

            controller.rb.AddForceAtPosition(transform.up * suspensionForce, transform.position);

            // Calculate steering force
            steeringForce = (-Vector3.Dot(transform.right, tireVelocity) * grip) / Time.fixedDeltaTime;

            controller.rb.AddForceAtPosition(transform.right * steeringForce, transform.position);

            if (isAccelerator)
            {
                // Calculate acceleration and braking force
                if (controller.verticalInput != 0f)
                {
                    float currentSpeed = Vector3.Dot(controller.rb.transform.forward, controller.rb.velocity);
                    float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentSpeed) / controller.maxSpeed);
                    torqueForce = controller.torqueCurve.Evaluate(normalizedSpeed) * controller.verticalInput;

                    controller.rb.AddForceAtPosition(transform.forward * torqueForce * controller.maxTorquePower, transform.position);
                }
            }
        }

        isGrounded = false;
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
