using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineBrain))]
public class SpectatorCamera : MonoBehaviour
{
    private List<Transform> cars = new();
    private CinemachineBrain brain;
    private Transform carToSpectate;

    public float switchCarInterval;

    void Start()
    {
        brain = GetComponent<CinemachineBrain>();

        foreach(Transform car in GameObject.Find("Cars").transform)
        {
            cars.Add(car);
        }

        carToSpectate = cars[0];
        cars.Remove(carToSpectate);
        SpectateCar();

        InvokeRepeating("ChangeCarToFollow", 0f, switchCarInterval);
    }

    private void ChangeCarToFollow()
    {
        int carIndex = Random.Range(0, cars.Count);

        cars.Add(carToSpectate);
        carToSpectate = cars[carIndex];
        cars.Remove(carToSpectate);

        SpectateCar();
    }

    private void SpectateCar()
    {
        brain.ActiveVirtualCamera.LookAt = carToSpectate;
    }
}
