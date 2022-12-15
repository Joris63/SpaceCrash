using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class CubeAddon : MonoBehaviour
{
    private Rigidbody firstTargetRb;
    private Rigidbody secondTargetRb;
    private GameObject firstTarget;
    private GameObject secondTarget;
    private Vector3 targetPos;
    private Vector3 startPosition;
    private float timeToReachTarget = 1;
    private float t;
    private bool setup = false;
    private Transform owner;

    public SphereCollider sphereCollider;

    void Start()
    {
        t = 0;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (t <= 1) TravelToPos(); 
        else
        {
            PullTargets();
        }
    }

    private void TravelToPos()
    {
        t += Time.deltaTime / timeToReachTarget;
        transform.position = Vector3.Lerp(startPosition, targetPos, t);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (t >= 1) Destroy(gameObject);
    }

    private void PullTargets()
    {
        firstTargetRb.AddForce((targetPos - firstTarget.transform.position).normalized * 100000);
        secondTargetRb.AddForce((targetPos - secondTarget.transform.position).normalized * 100000);
    }

    public void SetUpPull(Transform owner, GameObject firstTarget, GameObject secondTarget, Vector3 targetPos)
    {
        this.owner = owner;
        this.firstTargetRb = firstTarget.GetComponentInChildren<Rigidbody>();
        this.secondTargetRb = secondTarget.GetComponentInChildren<Rigidbody>();
        this.firstTarget = firstTarget;
        this.secondTarget = secondTarget;
        this.targetPos = targetPos;
    }
}
