using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CloseIndicators : MonoBehaviour
{
    private UIController uiControl;
    private CarController carController;
    private List<Transform> carTransforms;

    public float indatorAmount = 3;

    // Start is called before the first frame update
    void Awake()
    {
        uiControl = GetComponent<UIController>();
        carController = GetComponent<CarController>();
        for (int i = 0; i < carController.transform.parent.childCount; i++)
        {
            carTransforms.Add(carController.transform.parent.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void SortCarsDistance()
    {
        
    }
}
