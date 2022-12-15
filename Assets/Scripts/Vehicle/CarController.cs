using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("General Settings")]
    public float maxSpeed = 20f;
    public float maxTorquePower = 60f;
    public AnimationCurve torqueCurve;
    public WheelAnchor[] wheelAnchors;
    [SerializeField] private List<WheelAnchor> steeringAnchors;
    public float maxSteerAngle;
    public AnimationCurve steerAngleLimiter = AnimationCurve.Linear(0.0f, 1f, 200.0f, 0.3f);
    public float steerAngleChangeSpeed;
    public LayerMask ground;
    public Transform centerOfMass;
    public bool isBot = false;

    [Header("Aerial Settings")]
    public float stabilizationTorque = 100f;

    [Header("Other Settings")]
    public float downForce = 5f;
    public float flipDuration = .8f;

    [Header("Debug Settings")]
    public bool useSinglePlayerInputs = false;

    // ---------------
    // Public variables
    public PlayerController player { get; set; }
    public Rigidbody rb { get; private set; }
    public CarHealth health { get; private set; }
    public bool driveable { get; set; } = true;
    public bool targetable { get; set; }
    public bool isDestroyed { get; set; } = false;
    public bool isGrounded { get; private set; } = false;
    public Vector3 downDirection { get; private set; }
    public float currentSpeed { get; private set; } // magnitude of vehicle
    public float boostDeactivateTime { get; set; }
    public float timeAlive { get; private set; } = 0;

    // ---------------
    // Private variables
    private CarAI ai;
    private LevelManager levelManager;
    private ParticleSystem ps;
    private Vector3 originalCenterOfMass;

    private float carAngle;
    private float currentSteerAngle = 0;

    // ---------------
    // Input variables & update method
    public float verticalInput { get; set; }
    public float horizontalInput { get; set; }
    public bool unflipCarInput { get; set; }

    // ---------------
    // Ability variables
    public bool isTargetable { get; set; } = true;
    public List<CarCanvas> carCanvasRefs { get; set; } = new();


    // Update control variables
    public void UpdateControls(float horizontal, float vertical, bool unflip)
    {
        verticalInput = vertical;
        horizontalInput = horizontal;
        unflipCarInput = unflip;
    }
    // ---------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<CarHealth>();
        ps = GetComponentInChildren<ParticleSystem>();
        levelManager = FindObjectOfType<LevelManager>();

        // Set center of mass
        originalCenterOfMass = rb.centerOfMass;
        rb.centerOfMass = centerOfMass.localPosition;

        if (health)
            health.onDestroyed.AddListener(OnDestroyed);
    }

    private void OnDisable()
    {
        if (health)
            health.onDestroyed.AddListener(OnDestroyed);
    }

    private void Start()
    {
        if (isBot)
        {
            ai = gameObject.AddComponent<CarAI>();
            ai.InitializeAI();
        }

        if (PlayerManager.main != null)
        {
            useSinglePlayerInputs = false;
        }
    }

    private void Update()
    {
        downDirection = -(ArenaController.main.transform.position - transform.position).normalized;

        Debug.DrawRay(ArenaController.main.transform.position, downDirection * 10, Color.blue);

        if (ps)
        {
            ps.transform.localRotation = Quaternion.LookRotation(ps.transform.parent.InverseTransformDirection(Vector3.up), ps.transform.up);
        }

        if (isDestroyed)
        {
            return;
        }

        timeAlive += Time.deltaTime;

        if (useSinglePlayerInputs && !isBot)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool unflip = Input.GetKeyDown(KeyCode.Q);

            UpdateControls(horizontal, vertical, unflip);
        }

        float targetSteerAngle = horizontalInput * maxSteerAngle;
        float steerAngleMultiplier = steerAngleLimiter.Evaluate(currentSpeed * 3.6f / 100f);

        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, steerAngleMultiplier * targetSteerAngle, Time.deltaTime * steerAngleChangeSpeed);

        // Set rotation of steering anchors based on horizontal input
        foreach (WheelAnchor anchor in steeringAnchors)
        {
            anchor.transform.localEulerAngles = new Vector3(0, currentSteerAngle, 0);
        }

        // Slow down the car if no vertical input is present
        rb.drag = verticalInput == 0 ? 1f : 0f;

        carAngle = Vector3.Dot(transform.up, downDirection);

    }

    private void FixedUpdate()
    {
        // Set current speed
        currentSpeed = rb.velocity.magnitude;

        if (isDestroyed || !driveable)
        {
            return;
        }

        isGrounded = CheckIfGrounded();

        HandleMove();
    }

    private bool CheckIfGrounded(bool fullyGrounded = true)
    {
        bool result = fullyGrounded;

        foreach (WheelAnchor wheel in wheelAnchors)
        {
            if ((!wheel.isGrounded && fullyGrounded) || (wheel.isGrounded && !fullyGrounded))
            {
                result = !fullyGrounded;
                break;
            }
        }

        return result;
    }

    private bool CheckFalling()
    {
        float fallVelocity = Vector3.Dot(rb.velocity, downDirection);

        if (Mathf.Approximately(fallVelocity, 0))
            fallVelocity = 0f;

        if (fallVelocity > 0)
        {
            return true;
        }

        return false;
    }

    private void CapSpeed()
    {
        float speed = rb.velocity.magnitude;

        Debug.Log(speed);
        if (speed > maxSpeed && boostDeactivateTime < timeAlive)
        {
            Debug.Log("reduce speed");
            rb.velocity -= 0.01f * rb.velocity;
        }
    }

    private void HandleMove()
    {
        StabilizeCar();
        CapSpeed();

        if (!isGrounded && carAngle > .85f && unflipCarInput)
        {
            StartCoroutine(FlipCar());
        }

        AddDownForce();
    }

    private void StabilizeCar()
    {
        if (!CheckIfGrounded(false) && CheckFalling())
        {
            Vector3 locUp = transform.up;
            Vector3 wsUp = -downDirection;
            Vector3 axis = Vector3.Cross(locUp, wsUp);
            float force = stabilizationTorque;

            rb.AddTorque(axis * force);
        }
    }

    private IEnumerator FlipCar()
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position - downDirection * 1.5f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 0, 180);

        bool reachedTargetPosition = false;

        while (timeElapsed < flipDuration)
        {
            if (!reachedTargetPosition)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / (flipDuration / 1.5f));
                reachedTargetPosition = transform.position == targetPosition;
            }

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / flipDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void OnDestroyed()
    {
        isDestroyed = true;
        rb.centerOfMass = originalCenterOfMass;

        if (GameManager.main != null)
        {
            GameManager.main.OnCarDeath(gameObject, health.lastCollider);
        }

        Transform canvasContainer = gameObject.transform.Find("UI");
        for (int i = 0; i < canvasContainer.childCount; i++)
        {
            Destroy(canvasContainer.GetChild(i).gameObject);
        }

        if (ps)
            ps.Play();

        foreach (WheelAnchor anchor in wheelAnchors)
        {
            PopOffWheel(anchor);
        }
    }

    private void PopOffWheel(WheelAnchor anchor)
    {
        Transform wheelContainer = levelManager != null ? levelManager.wheelContainer : null;

        anchor.wheelView.SetParent(wheelContainer, true);

        Rigidbody wheelRb = anchor.wheelView.gameObject.AddComponent<Rigidbody>();
        MeshCollider wheelMesh = anchor.wheelView.GetComponentInChildren<MeshRenderer>().gameObject.AddComponent<MeshCollider>();

        wheelRb.mass = 50;
        wheelRb.useGravity = false;
        wheelRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        wheelMesh.convex = true;

        ArenaController.main.AddRigidbody(wheelRb);

        wheelRb.AddForce((UnityEngine.Random.onUnitSphere.normalized * 10) + rb.velocity, ForceMode.VelocityChange);
    }

    // this is used to add more grip in relation to speed
    private void AddDownForce()
    {
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);
    }
}