using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadEffect : ScriptableObject
{
    public Material material;

    [ColorUsage(true, true)]
    public Color color;

    public virtual void ApplyEffect(Rigidbody car) 
    {
        Debug.Log("Apply Effect");
    }
}
