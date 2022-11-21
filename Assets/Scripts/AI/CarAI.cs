using UnityEngine;
using System.Collections.Generic;

public class CarAI : StateMachine
{
    [HideInInspector] public List<CarController> cars = new List<CarController>();

    // AI Atrributes
    [Header("Attributes")]
    [Range(0, 1)]
    [Tooltip("How eager the AI is to keep pursuing. 0 means it rather idles, 1 means it rather pursues.")]
    public float aggression = 0.25f;

    // Blackboard Variables
    [HideInInspector] public bool hitOpponent = false;
    [HideInInspector] public float idleTime;
    [HideInInspector] public float useAbilityCooldown;

    // States
    [HideInInspector] public BaseState pursuing;
    [HideInInspector] public BaseState reversing;
    [HideInInspector] public BaseState idle;

    private CarController controller;

    public void InitializeAI()
    {
        controller = GetComponent<CarController>();

        pursuing = new Pursuing(this);
        reversing = new Reversing(this);
        idle = new Idle(this);

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            cars.Add(transform.parent.GetChild(i).GetComponent<CarController>());
        }

        useAbilityCooldown = Random.Range(3f, 10f);

        int rndmStateNmbr = Random.Range(1, 4);
        if (rndmStateNmbr == 3)
        {
            idleTime = Random.Range(5f, 10f);
            Initialize(idle);
        }
        else
        {
            Initialize(pursuing);
        }
    }

    public void SetVertical(float v)
    {
        controller.verticalInput = v;
    }

    public void SetHorizontal(float h)
    {
        controller.horizontalInput = h;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent == transform.parent && currentState != reversing)
        {
            bool isPossibleAttacker = Mathf.Abs(Vector3.SignedAngle(transform.forward, (collision.transform.position - transform.position).normalized, Vector3.up)) <= 60f;
            hitOpponent = isPossibleAttacker;
        }
    }
}
