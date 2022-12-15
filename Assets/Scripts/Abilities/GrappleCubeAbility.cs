using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Abilities/GrappleCube")]

public class GrappleCubeAbility : Ability
{
    public GameObject PullCube;


    private GameObject firstTarget;
    private GameObject secondTarget;
    private Transform gunTip;
    private Vector3 targetPos = new Vector3();
    private float cooldown = 5f;

    private GameObject closestCar;
    private GameObject lastTarget;
    public LayerMask carMask;

    public float range;
    public float angle = 60;

    private MeshCollider targetCollider;
    private RaycastHit rh;

    private bool ready;

    public override void Obtained()
    {
        base.Obtained();

        ready = true;
        gunTip = abilityController.transform.Find("GunTip");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        GetClosestTwoCars();

        bool targets = false;
        if (firstTarget && secondTarget) targets = true;

        if (Input.GetKey(KeyCode.E) && !carController.isDestroyed && cooldown < 0f)
        {
            cooldown = 5f;
            Activated();
        }

        cooldown -= Time.deltaTime;
        /*
        if (abilityController.hud)
        {
            if (targets) abilityController.hud.TargetingTextEnable();
            else abilityController.hud.TargetingTextDisable();
        }
        */
    }

    public override void Activated()
    {
        base.Activated();

        LaunchPull();
    }

    public override void CarDestroyed()
    {
        base.CarDestroyed();

        AbilityEnded(true);
    }

    private void AbilityEnded(bool isDestroyed)
    {
        if (!isDestroyed) abilityController.AbilityEnded();
    }

    private void LaunchPull()
    {
        if(ready)
        {
            GetClosestTwoCars();
            targetPos = Vector3.Lerp(firstTarget.transform.position, secondTarget.transform.position, 0.5f);

            Vector3 dirToTarget = targetPos.normalized;

            Quaternion throwRotation = Quaternion.Euler(dirToTarget);

            GameObject projectile = Instantiate(PullCube, gunTip.position, throwRotation);
            projectile.transform.parent = abilityController.abilityContainer;

            CubeAddon projectileScript = projectile.GetComponentInChildren<CubeAddon>();
            projectileScript.SetUpPull(carController.transform, firstTarget, secondTarget, targetPos + Vector3.up);
        }
    }

    private void GetClosestTwoCars()
    {
        float closestDistance = 999f;
        float secondClosestDistance = 999f;


        for (int i = 0; i < carController.transform.parent.childCount; i++)
        {
            Transform car = carController.transform.parent.GetChild(i);
            if (car == carController.transform) continue;

            Vector3 direction = (car.transform.position - carController.transform.position).normalized;
            float angleT = Vector3.Angle(carController.transform.forward, direction);
            bool isWithinView = Vector3.Angle(carController.transform.forward, direction) <= angle;

            float distance = Vector3.Distance(car.transform.position, carController.transform.position);
            if (isWithinView && distance < range)
            {
                Physics.Raycast(carController.transform.position, direction, out rh, distance, carMask);
                if (!Physics.Raycast(carController.transform.position, direction, distance, carMask))
                {
                    if(distance < closestDistance)
                    {
                        secondClosestDistance = closestDistance;
                        closestDistance = distance;
                        secondTarget = firstTarget;
                        firstTarget = car.gameObject;
                    }
                    else if(distance < secondClosestDistance)
                    {
                        secondClosestDistance = closestDistance;
                        secondTarget = car.gameObject;
                    }
                }
            }
        }
    }
}
