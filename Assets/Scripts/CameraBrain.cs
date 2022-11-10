using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBrain : MonoBehaviour
{
    private CinemachineVirtualCamera currentlyActiveCamera;
    [Tooltip("The first camera in the list will be the default on game start")]
    [SerializeField]
    private CinemachineVirtualCamera[] virtualCameras;


    private void Start()
    {
        SetActiveCamera(virtualCameras[0]);
    }

    private void FixedUpdate()
    {
        if (GameManager.main.isOutsideField && currentlyActiveCamera.name != "Car cam Outside")
        {
            SetActiveCamera("Car Cam Outside");
        }
        else if (currentlyActiveCamera.name != "Car Cam Inside")
        {
            SetActiveCamera("Car Cam Inside");
        }
    }

    private void updateCameraPriority(CinemachineVirtualCamera camera)
    {
        if (currentlyActiveCamera != null)
        {
            camera.Priority = currentlyActiveCamera.Priority + 1;
            currentlyActiveCamera.Priority--;
        }
        else
        {
            camera.Priority = 11;
        }

        currentlyActiveCamera = camera;
    }

    public void SetActiveCamera(string cameraName)
    {
        foreach (CinemachineVirtualCamera cam in virtualCameras.AsSpan())
        {
            if (cam.name == cameraName)
            {
                updateCameraPriority(cam);
            }
        }
    }

    public void SetActiveCamera(CinemachineVirtualCamera camera)
    {
        updateCameraPriority(camera);
    }

}
