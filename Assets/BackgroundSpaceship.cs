using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpaceship : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    bool generated = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!generated)
        {

            animator.SetInteger("animation", Random.Range(0, 2));
            generated = true;
        }
    }

    public void animation_end()
    {
        generated = false;
    }
}
