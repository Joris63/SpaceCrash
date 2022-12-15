using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent onCarsInitialized;

    [SerializeField]
    private Transform Arena;

    [SerializeField]
    private Transform CarContainer;

    [SerializeField]
    private Transform SpawnPointParentTransform;
    private List<Transform> SpawnPoints = new List<Transform>();

    public GameObject CarPrefab;

    private void Awake()
    {
        if (SpawnPointParentTransform != null)
        {
            for (int i = 0; i < SpawnPointParentTransform.childCount; i++)
            {
                SpawnPoints.Add(SpawnPointParentTransform.GetChild(i).GetComponent<Transform>());
            }
        }

        foreach (Transform spawnPoint in SpawnPoints)
        {
            SpawnCar(spawnPoint);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool first = true;
    private void SpawnCar(Transform spawnPoint)
    {
        CarController car = Instantiate(CarPrefab, spawnPoint.position, spawnPoint.rotation, CarContainer).GetComponent<CarController>();
        car.driveable = first;
        car.isBot = first;

        first = false;
    }
}
