using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
    private CarController car;
    private bool isDrifting = false;
    private bool tireMarksFlag;
    public TrailRenderer rearLeftRenderer;
    public TrailRenderer rearRightRenderer;


    private CarSound carSound = null;

    // Start is called before the first frame update
    void Start()
    {
        car = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (carSound == null) return;
        CheckDrift();
        //UpdateTrailPosition();
    }

    public void SetSound(CarSound carSound)
    {
        this.carSound = carSound;
    }
    public void UpdateTrailPosition()
    {
        WheelHit leftHit;
        WheelHit rightHit;
    }

    private void CheckDrift()
    {
        
    }

    private void StartEmitter()
    {
        if (tireMarksFlag)
        {
            return;
        }

        rearLeftRenderer.emitting = true;
        rearRightRenderer.emitting = true;

        tireMarksFlag = true;
    }

    private void StopEmitter()
    {
        if (!tireMarksFlag)
        {
            return;
        }

        rearLeftRenderer.emitting = false;
        rearRightRenderer.emitting = false;

        tireMarksFlag = false;
    }
}
