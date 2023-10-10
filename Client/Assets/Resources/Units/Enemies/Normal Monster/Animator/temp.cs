using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("Damage");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetTrigger("Attack1");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Attack2");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("Attack3");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetTrigger("Attack4");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetTrigger("Attack5");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetTrigger("Attack6");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Attack7");
        }
    }
}
